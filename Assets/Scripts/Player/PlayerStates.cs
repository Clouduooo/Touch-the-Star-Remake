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

    public IdleState(PlayerFSM manager)  //构造函数中获得状态机和玩家数据的引用
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        //TODO:播放动画
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if(parameter.inputHandler.MovementInput.y != 0)
        {
            manager.TransitionState(StateType.Jump);
        }
        else if(parameter.inputHandler.MovementInput.x != 0)     //如果按下移动按钮
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

    public MoveState(PlayerFSM manager)  //构造函数中获得状态机和玩家数据的引用
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        DetectMove();
        if(parameter.inputHandler.MovementInput.y != 0)
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
            parameter.extendDuration = 2f;      //设定横向移动时光圈扩散时间长度
            GameObject.Instantiate(parameter.circleLoop, parameter.rayPoint_front.transform.position, Quaternion.identity);
            parameter.initCD = 0;
        }

        if (parameter.canMove)
        {
            //地面上横向移动逻辑
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

    public JumpState(PlayerFSM manager)  //构造函数中获得状态机和玩家数据的引用
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        DetectJump();
        if (!parameter.canJump)
        {
            if(parameter.canMove)    manager.TransitionState(StateType.Move);
            else    manager.TransitionState(StateType.Idle);
        }
    }

    void DetectJump()
    {
        if (parameter.canJump)
        {
            parameter.catHead.SetActive(true);
            //parameter.catHead.transform.position = Vector2.MoveTowards(manager.gameObject.transform.position, parameter.jumpPos, );
        }
    }
}
#endregion