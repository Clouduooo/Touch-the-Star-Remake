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

[Serializable]
public class PlayerParameter        //�������
{
    public bool canMove;
    public bool canJump;
    public Rigidbody2D rb;
    public Vector2 speed;            //�������speed���ۺ�������ҳ�����ٶ�����
    public float moveTime;
    public float direction;
    public AnimationCurve moveCurve;
    public PlayerInputHandler inputHandler;
    public Animator animator;
    public GameObject catHead;
    public Vector2 jumpPos;

    //д���������޸����--��������
    public GameObject circleLoop;

    //���λ�Ʋ���--�����ж���Ȧ����ʱ��
    public float extendDuration;
    public float initCD;
    public float cdTime;

    //��ײ������--��Ҫ��Inspector��ֵ
    public Transform rayPoint_front;
    public Transform rayPoint_back;
    public float radius;
    public LayerMask lightLayer;
    public Tilemap lightTile;
    public BoundsInt bounds;       //tilemap�ı߽�
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

        //��������
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.inputHandler = GetComponent<PlayerInputHandler>();
        Debug.Log(parameter.inputHandler);
        parameter.animator = GetComponentInChildren<Animator>();

        //��ȡ��Ƭ��ͼ�߽������ ����ʼ������lightTile�е���ƬΪ͸��
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

        TransitionState(StateType.Idle);    //���ó�ʼ״̬ΪIdle

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
        if (parameter.direction != 0)
        {
            gameObject.transform.localScale = new Vector3(-parameter.direction, 1, 1);
        }
    }

    void AddSpeed()
    {
        parameter.rb.velocity = parameter.speed;
    }

    //ֻҪèè��������ײ���ұ������Ϳ��Ժ����ƶ�����������ƶ�
    void RacastCheck()
    {
        //�����ƶ����
        if (!Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer) && Physics2D.OverlapCircle((Vector2)parameter.rayPoint_back.position, parameter.radius, parameter.lightLayer))
        {
            parameter.canMove = false;
        }
        //else if(Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer))
        //{
        //    //Debug.Log("Check Front!");
        //    if(Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer).gameObject.GetComponent<Tilemap>().GetColor(new Vector3Int((int)Mathf.Round(parameter.rayPoint_front.position.x), (int)Mathf.Round(parameter.rayPoint_front.position.y), 0)).a < 1f)
        //    {
        //        parameter.canMove = false;
        //    }
        //}
        else
        {
            parameter.canMove = true;
        }

        ////��Ծ���
        //if (Physics2D.Raycast((Vector2)transform.position, Vector2.up).collider.gameObject.GetComponent<SpriteRenderer>().color.a <= 0.7)
        //{
        //    parameter.canJump = false;
        //}
        //else
        //{
        //    parameter.jumpPos = Physics2D.Raycast((Vector2)transform.position, Vector2.up).collider.gameObject.transform.position;
        //    parameter.canJump = true;
        //}
    }

    void OnDrawGizmos()    //��ʾ���߲���
    {
        Gizmos.DrawWireSphere(parameter.rayPoint_front.position, parameter.radius);
        Gizmos.DrawWireSphere(parameter.rayPoint_back.position, parameter.radius);
    }
}
