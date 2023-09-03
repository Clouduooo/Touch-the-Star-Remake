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

    //��ײ������--��Ҫ��Inspector��ֵ
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

        TransitionState(StateType.Idle);    //���ó�ʼ״̬ΪIdle

        parameter.catHead.SetActive(false);

        //��������
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

    //ֻҪèè��������ײ���ұ������Ϳ��Ժ����ƶ�����������ƶ�
    void RacastCheck()
    {
        //�����ƶ����
        if (!Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer) && Physics2D.OverlapCircle((Vector2)parameter.rayPoint_back.position, parameter.radius, parameter.lightLayer) ||
            Physics2D.OverlapCircle((Vector2)parameter.rayPoint_front.position, parameter.radius, parameter.lightLayer).gameObject.GetComponent<SpriteRenderer>().color.a != 1)
        {
            parameter.canMove = false;
        }
        else
        {
            parameter.canMove = true;
        }

        //��Ծ���
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

    void OnDrawGizmos()    //��ʾ���߲���
    {
        Gizmos.DrawWireSphere(parameter.rayPoint_front.position, parameter.radius);
        Gizmos.DrawWireSphere(parameter.rayPoint_back.position, parameter.radius);
    }
}
