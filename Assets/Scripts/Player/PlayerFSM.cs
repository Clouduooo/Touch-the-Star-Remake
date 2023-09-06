using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public enum StateType
{
    Idle, Move, Jump
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
    public Vector2 jumpPos;
    public bool jumpFinished;   //Use to change state;
    public bool inJumpState;    //Use to forbid the FaceFlip
    public GameObject leftShape, rightShape;

    //The direction of platform player is on
    public PlatformDirType platformDir;

    //test
    public GameObject circleLoop;

    //test for instantiate circle loop
    public float extendDuration;
    public float initCD;
    public float cdTime;

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
        states.Add(StateType.Jump, new JumpState(this));

        parameter.catHead.SetActive(false);
        parameter.leftShape.SetActive(false);
        parameter.rightShape.SetActive(false);

        //Attach to player's component
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.inputHandler = GetComponent<PlayerInputHandler>();
        Debug.Log(parameter.inputHandler);
        parameter.animator = GetComponentInChildren<Animator>();

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
        AddSpeed();
    }

    void Update()
    {
        parameter.initCD += Time.deltaTime; 
        FlipDirection();
        RacastCheck();
        currentState.OnUpdate();
    }

    public void TransitionState(StateType type)
    {
        if(currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipDirection()
    {
        if (parameter.direction != 0 && !parameter.inJumpState)
        {
            switch (parameter.platformDir)
            {
                case PlatformDirType.Up:
                    gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
                    break;
                case PlatformDirType.Down:
                    gameObject.transform.localScale = new Vector3(parameter.direction, 1, 1);
                    break;
                //case PlatformDirType.Left:
                //    gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
                //    break;
                //case PlatformDirType.Right:
                //    gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
                //    break;
            }
        }
    }

    void AddSpeed()
    {
        parameter.rb.velocity = parameter.speed;
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

    void OnDrawGizmos()    //Show raycast in editor mode
    {
        Gizmos.DrawWireSphere(parameter.rayPoint_front.position, parameter.radius);
        Gizmos.DrawWireSphere(parameter.rayPoint_back.position, parameter.radius);
    }
}
