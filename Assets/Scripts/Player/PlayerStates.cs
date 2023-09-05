using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

#region IdleState
public class IdleState : IPlayerState
{
    private PlayerFSM manager;
    private PlayerParameter parameter;

    public IdleState(PlayerFSM manager)  //���캯���л��״̬����������ݵ�����
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        //TODO:���Ŷ���
        parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if(parameter.inputHandler.jumpDir != JumpInput.None)
        {
            manager.TransitionState(StateType.Jump);
        }
        else if(parameter.inputHandler.MovementInput.x != 0)     //��������ƶ���ť
        {
            manager.TransitionState(StateType.Move);
        }
    }
}
#endregion

#region MoveState
public class MoveState : IPlayerState
{
    private PlayerFSM manager;
    private PlayerParameter parameter;

    public MoveState(PlayerFSM manager)  //���캯���л��״̬����������ݵ�����
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        parameter.speed = new Vector2(0, 0);
    }

    public void OnUpdate()
    {
        DetectMove();
        if(parameter.inputHandler.jumpDir != JumpInput.None)
        {
            manager.TransitionState(StateType.Jump);
        }
        else if(parameter.speed.x == 0)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    void DetectMove()
    {
        if(parameter.inputHandler.MovementInput.x > 0)
        {
            parameter.direction = 1;
        }
        else if(parameter.inputHandler.MovementInput.x < 0)
        {
            parameter.direction = -1;
        }
        else
        {
            parameter.direction = 0;
        }

        if(parameter.direction != 0 && parameter.initCD >= parameter.cdTime)
        {
            parameter.extendDuration = 2f;      //�趨�����ƶ�ʱ��Ȧ��ɢʱ�䳤��
            GameObject.Instantiate(parameter.circleLoop, parameter.rayPoint_front.transform.position, Quaternion.identity);
            parameter.initCD = 0;
        }

        if (parameter.canMove)
        {
            //�����Ϻ����ƶ��߼�
            if (parameter.direction != 0 && parameter.moveTime < 0.6f)
            {
                parameter.speed = new Vector2(parameter.moveCurve.Evaluate(parameter.moveTime) * parameter.direction, 0);
                parameter.moveTime += Time.deltaTime;
            }else if(parameter.direction == 0)
            {
                parameter.moveTime = 0;
                parameter.speed = new Vector2(0, 0);
            }
        }
        else
        {
            parameter.moveTime = 0;
            parameter.speed = new Vector2(0, 0);
        }
    }
}
#endregion

#region JumpState
public class JumpState : IPlayerState
{
    private PlayerFSM manager;
    private PlayerParameter parameter;

    public JumpState(PlayerFSM manager)  //���캯���л��״̬����������ݵ�����
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        Jump();
    }

    public void OnExit()
    {
        parameter.catHead.SetActive(false);
        parameter.jumpFinished = false;
    }

    public void OnUpdate()
    {
        if (parameter.jumpFinished)
        {
            if (parameter.canMove)
            {
                parameter.inputHandler.jumpDir = JumpInput.None;
                manager.TransitionState(StateType.Move);
            }
            else
            {
                parameter.inputHandler.jumpDir = JumpInput.None;
                manager.TransitionState(StateType.Idle);
            }
        }
    }

    void Jump()
    {
        parameter.animator.Play("Jump_Prepare");
        parameter.catHead.SetActive(true);
    }
}
#endregion