using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

#region IdleState
public class IdleState : IPlayerState
{
    private PlayerFSM manager;
    private PlayerParameter parameter;

    public IdleState(PlayerFSM manager)  //Get reference of PlayerFSM and Player Data from Initiate Function
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        //TODO:Play animation
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
        else if(parameter.inputHandler.MovementInput.x != 0)     //if press move button
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

    public MoveState(PlayerFSM manager)  //Get reference of PlayerFSM and Player Data from Initiate Function
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
            parameter.extendDuration = 2f;      //Set time duration of circle loop while moving on platform
            GameObject.Instantiate(parameter.circleLoop, parameter.rayPoint_front.transform.position, Quaternion.identity);
            //GameObject.Instantiate(parameter.circleLoop, manager.transform.position, Quaternion.identity);
            parameter.initCD = 0;
        }

        if (parameter.canMove)
        {
            //Moving logic on platform
            if (parameter.direction != 0 && parameter.moveTime < 0.6f)
            {
                parameter.speed = new Vector2(parameter.moveCurve.Evaluate(parameter.moveTime) * parameter.direction * 1.5f, 0);
                //parameter.speed = new Vector2(55f * parameter.direction, 0);
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

    public JumpState(PlayerFSM manager)  //Get reference of PlayerFSM and Player Data from Initiate Function
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
        parameter.inJumpState = false;
    }

    public void OnUpdate()
    {
        if (parameter.jumpFinished)
        {
            if (parameter.inputHandler.MovementInput.x != 0)
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
        parameter.inJumpState = true;

        if (parameter.inputHandler.jumpDir == JumpInput.Up)
        {
            parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
            parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
            parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x, manager.transform.position.y + 16.5f, manager.transform.position.z);
            parameter.animator.Play("Jump_Prepare_Verticle");
        }
        else if(parameter.inputHandler.jumpDir == JumpInput.Down)
        {
            parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 180);
            parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
            parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x, manager.transform.position.y + 16.5f, manager.transform.position.z);
            parameter.animator.Play("Jump_Prepare_Verticle");
        }
        else if(parameter.inputHandler.jumpDir == JumpInput.Left)
        {
            manager.transform.localScale = new Vector3(1, 1, 1);
            parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
            parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
            parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x - 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
            parameter.animator.Play("Jump_Prepare_Horizontal");
        }
        else if (parameter.inputHandler.jumpDir == JumpInput.Right)
        {
            manager.transform.localScale = new Vector3(-1, 1, 1);
            parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
            parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
            Vector3 tempPos = parameter.leftShape.transform.position;
            parameter.leftShape.transform.position = parameter.rightShape.transform.position;
            parameter.rightShape.transform.position = tempPos;
            parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x + 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
            parameter.animator.Play("Jump_Prepare_Horizontal");
        }
        parameter.catHead.SetActive(true);
    }
}
#endregion