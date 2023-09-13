using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum StateType
{
    Idle, Move, Jump,JumpNew,
}

public enum PlatformDirType
{
    Up, Down, Left, Right
}

[Serializable]
public class PlayerParameter        //player's data
{
    public bool canMove;
    public Rigidbody2D rb;
    public Vector2 speed;            //speed with direction
    public float moveTime;
    public float direction;
    public AnimationCurve moveCurve;
    public PlayerInputHandler inputHandler;
    public Animator animator;
    public GameObject catHead;

    //jump related
    public bool jumpFinished;   //Use to change state;
    public bool inJumpState;    //Use to forbid the FaceFlip
    public GameObject leftShape, rightShape;
    public GameObject body;
    public RaycastHit2D jumpHit;
    public TileManager tileManager;
    public Transform jumpCheckRayStartPoint;
    public bool isJumpPreAnimFin;
    public bool isJumpRollingAnimFin;
    public AnimationCurve flyingCurve;

    public float flySpeedFix;       //fix the speed of flying/jumping

    //The direction of platform player is on
    public PlatformDirType platformDir;

    //ObjectPool related to use circle loop
    public float extendDuration;
    public bool isWalking;
    public float initCD;
    public float cdTime;
    public CircleObjectPool circleObjectPool;
    public float jumpDurtaionFix;               //fix the extendDuration of jump

    //Raycast Checking parameter
    public Transform rayPoint_front;
    public Transform rayPoint_back;
    public float radius;
    public LayerMask lightLayer;
    public Tilemap lightTile;
    public BoundsInt bounds;       //tilemap's bound
}

public class PlayerFSM : MonoBehaviour
{
    public PlayerParameter parameter;

    private IPlayerState currentState;
    private Dictionary<StateType, IPlayerState> states = new Dictionary<StateType, IPlayerState>();

    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Move, new MoveState(this));
        states.Add(StateType.JumpNew, new JumpStateNew(this));

        parameter.catHead.SetActive(false);
        // parameter.leftShape.SetActive(false);
        // parameter.rightShape.SetActive(false);
        parameter.body.SetActive(false);

        //Attach to player's component
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.inputHandler = GetComponent<PlayerInputHandler>();
        parameter.animator = GetComponentInChildren<Animator>();
        parameter.circleObjectPool = GetComponent<CircleObjectPool>();

        //Initialize all tiles with alpha = 0
        parameter.bounds = parameter.lightTile.cellBounds;
        for (int x = parameter.bounds.xMin; x <= parameter.bounds.xMax; x++)
        {
            for (int y = parameter.bounds.yMin; y < parameter.bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                //Color color = new Color(1, 1, 1, 0);
                Color color = parameter.lightTile.GetColor(pos);
                color.a=0;
                parameter.lightTile.SetTileFlags(pos, TileFlags.None);
                parameter.lightTile.SetColor(pos, color);
            }
        }

        TransitionState(StateType.Idle);    //Initialize the first state--Idle

        //test
        parameter.initCD = parameter.cdTime;
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
        if(!parameter.inJumpState)
            AddSpeed();
    }

    void Update()
    {
        parameter.initCD += Time.deltaTime; 
        AdjustInput();
        FlipDirection();
        RacastCheck();
        currentState.OnUpdate();
    }

    public void TransitionState(StateType type)
    {
        currentState?.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipDirection()
    {
        if (parameter.direction != 0 && !parameter.inJumpState)
        {
            gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
            // switch (parameter.platformDir)
            // {
            //     case PlatformDirType.Up:
            //         gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
            //         break;
            //     case PlatformDirType.Down:
            //         gameObject.transform.localScale = new Vector3(parameter.direction, 1, 1);
            //         break;
                //case PlatformDirType.Left:
                //    gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
                //    break;
                //case PlatformDirType.Right:
                //    gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
                //    break;
            //}
        }
    }

    void AddSpeed()
    {
        parameter.rb.velocity = transform.rotation*parameter.speed;
    }

    //Raycast Checking Function
    void RacastCheck()
    {
        //Move Checking
        if (!Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer) && Physics2D.OverlapCircle((Vector2)parameter.rayPoint_back.position, parameter.radius, parameter.lightLayer))
        {
            parameter.canMove = false;
        }
        else if(!Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer) && !Physics2D.OverlapCircle((Vector2)parameter.rayPoint_back.position, parameter.radius, parameter.lightLayer))
        {
            parameter.canMove = false;
        }
        else
        {
            parameter.canMove = true;
        }
    }

    void AdjustInput()
    {
        parameter.inputHandler.AdjustedMovementDir=Vector2Int.RoundToInt(Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,-transform.rotation.eulerAngles.z)*parameter.inputHandler.MovementInput);
        Vector2Int AdjustedJumpDir=new(0,0);
        switch(parameter.inputHandler.jumpDir)
        {
            case JumpInput.Up:
                AdjustedJumpDir=new(0,1);
                break;
            case JumpInput.Down:
                AdjustedJumpDir=new(0,-1);
                break;
            case JumpInput.Left:
                AdjustedJumpDir=new(-1,0);
                break;
            case JumpInput.Right:
                AdjustedJumpDir=new(1,0);
                break;
        }
        AdjustedJumpDir=Vector2Int.RoundToInt(Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,-transform.rotation.eulerAngles.z)*(Vector2)AdjustedJumpDir);
        parameter.inputHandler.AdjustedJumpDir = (AdjustedJumpDir.x, AdjustedJumpDir.y) switch
        {
            (0, 1) => JumpInput.Up,
            (0, -1) => JumpInput.Down,
            (-1, 0) => JumpInput.Left,
            (1, 0) => JumpInput.Right,
            _ => JumpInput.None,
        };
    }

    void OnDrawGizmos()    //Show raycast in editor mode
    {
        Gizmos.DrawWireSphere(parameter.rayPoint_front.position, parameter.radius);
        Gizmos.DrawWireSphere(parameter.rayPoint_back.position, parameter.radius);
    }
}
