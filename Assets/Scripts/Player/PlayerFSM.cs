using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum StateType
{
    Idle, Move, Jump
}

[Serializable]
public class PlayerParameter        //玩家数据
{
    public bool canMove;
    public bool canJump;
    public Rigidbody2D rb;
    public Vector2 speed;            //带方向的speed，综合玩家左右朝向和速度曲线
    public float moveTime;
    public float direction;
    public AnimationCurve moveCurve;
    public PlayerInputHandler inputHandler;
    public Animator animator;
    public GameObject catHead;
    public Vector2 jumpPos;

    //碰撞检测相关--需要在Inspector赋值
    public Transform rayPoint_front;
    public Transform rayPoint_back;
    public float radius;
    public LayerMask lightLayer;
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

        TransitionState(StateType.Idle);    //设置初始状态为Idle

        parameter.catHead.SetActive(false);

        //绑定玩家组件
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.inputHandler = GetComponent<PlayerInputHandler>();
        parameter.animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        AddSpeed();
    }

    void Update()
    {
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

    //只要猫猫附件有碰撞体且被点亮就可以横向移动，否则禁用移动
    void RacastCheck()
    {
        //地面移动检测
        if (!Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer) && Physics2D.OverlapCircle((Vector2)parameter.rayPoint_back.position, parameter.radius, parameter.lightLayer) ||
            Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer).gameObject.GetComponent<SpriteRenderer>().color.a != 1)
        {
            parameter.canMove = false;
        }
        else
        {
            parameter.canMove = true;
        }

        //跳跃检测
        if (Physics2D.Raycast((Vector2)transform.position, Vector2.up).collider.gameObject.GetComponent<SpriteRenderer>().color.a <= 0.7)
        {
            parameter.canJump = false;
        }
        else
        {
            parameter.jumpPos = Physics2D.Raycast((Vector2)transform.position, Vector2.up).collider.gameObject.transform.position;
            parameter.canJump = true;
        }
    }

    void OnDrawGizmos()    //显示射线参数
    {
        Gizmos.DrawWireSphere(parameter.rayPoint_front.position, parameter.radius);
        Gizmos.DrawWireSphere(parameter.rayPoint_back.position, parameter.radius);
    }
}
