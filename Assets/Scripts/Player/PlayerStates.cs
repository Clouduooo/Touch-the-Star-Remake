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
        if(parameter.inputHandler.AdjustedJumpDir != JumpInput.None)
        {
            manager.TransitionState(StateType.Jump);
        }
        else if(parameter.inputHandler.AdjustedMovementDir.x != 0)     //if press move button
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
        parameter.animator.Play("Run");
    }

    public void OnExit()
    {
        parameter.speed = new Vector2(0, 0);
    }

    public void OnUpdate()
    {
        DetectMove();
        if(parameter.inputHandler.AdjustedJumpDir != JumpInput.None)
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
        if(parameter.inputHandler.AdjustedMovementDir.x > 0)
        {
            parameter.direction = 1;
        }
        else if(parameter.inputHandler.AdjustedMovementDir.x < 0)
        {
            parameter.direction = -1;
        }
        else
        {
            parameter.direction = 0;
        }

        if(parameter.direction != 0 && parameter.initCD >= parameter.cdTime)
        {
            parameter.extendDuration = 0.8f;      //Set time duration of circle loop while moving on platform
            //GameObject.Instantiate(parameter.circleLoop, parameter.rayPoint_front.transform.position, Quaternion.identity);
            //GameObject.Instantiate(parameter.circleLoop, manager.transform.position, Quaternion.identity);
            // Use ObjectPool--Get Obeject!
            parameter.circleObjectPool.C_Pool.Get();
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
            }
            else if(parameter.direction != 0 && parameter.moveTime >= 0.6f)
            {
                parameter.speed = new Vector2(parameter.moveCurve.Evaluate(1f) * parameter.direction * 1.5f, 0);
            }
            else if(parameter.direction == 0)
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

    readonly private Vector3 vertStartPos=new(0,16.5f,0);
    readonly private Vector3 rightStartPos=new(12.6f,6.2f,0);
    readonly private Vector3 leftStartPos=new(-12.6f,6.2f,0);

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
            if (parameter.inputHandler.AdjustedMovementDir.x != 0)
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

        if (parameter.inputHandler.AdjustedJumpDir == JumpInput.Down)
        {
            parameter.jumpFinished = true;      //change state
        }

        switch (parameter.inputHandler.AdjustedJumpDir)
        {
            case JumpInput.Up:
                if(Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.up, Mathf.Infinity, parameter.lightLayer) &&
                    parameter.tileManager.SearchColor(new Vector3(Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.up, Mathf.Infinity, parameter.lightLayer).point.x, Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.up, Mathf.Infinity, parameter.lightLayer).point.y, 0)))
                {
                    parameter.jumpHit = Physics2D.Raycast((Vector2)manager.transform.position, Vector2.up, Mathf.Infinity, parameter.lightLayer);
                    //Time.timeScale = 0f;
                    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
                    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position + manager.transform.rotation * vertStartPos;
                    parameter.animator.Play("Jump_Prepare_Verticle");
                    parameter.catHead.SetActive(true);
                    Debug.Log(parameter.jumpHit.point);
                }
                else
                {
                    parameter.jumpFinished = true;
                }
                break;

            case JumpInput.Left:
                if(Physics2D.Raycast((Vector2)manager.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer) &&
                    parameter.tileManager.SearchColor(new Vector3(Physics2D.Raycast((Vector2)manager.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer).point.x, Physics2D.Raycast((Vector2)manager.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer).point.y, 0)))
                {
                    parameter.jumpHit = Physics2D.Raycast((Vector2)manager.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                    //Time.timeScale = 0f;
                    manager.transform.localScale = new Vector3(1, 1, 1);
                    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
                    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x - 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
                    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position + manager.transform.rotation * leftStartPos;
                    parameter.animator.Play("Jump_Prepare_Horizontal");
                    parameter.catHead.SetActive(true);
                    Debug.Log(parameter.jumpHit.point);
                }
                else
                {
                    parameter.jumpFinished = true;
                }
                break;

            case JumpInput.Right:
                if (Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer) &&
                    parameter.tileManager.SearchColor(new Vector3(Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer).point.x, Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer).point.y, 0)))
                {
                    parameter.jumpHit = Physics2D.Raycast((Vector2)manager.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                    //Time.timeScale = 0f;
                    manager.transform.localScale = new Vector3(-1, 1, 1);
                    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
                    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x + 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
                    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position + manager.transform.rotation * rightStartPos;
                    parameter.animator.Play("Jump_Prepare_Horizontal");
                    parameter.catHead.SetActive(true);
                    Debug.Log(parameter.jumpHit.point);
                }
                else
                {
                    parameter.jumpFinished = true;
                }
                break;
        }

        //if (parameter.inputHandler.AdjustedJumpDir == JumpInput.Up)
        //{
        //    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
        //    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x, manager.transform.position.y + 16.5f, manager.transform.position.z);
        //    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position+manager.transform.rotation*vertStartPos;
        //    parameter.animator.Play("Jump_Prepare_Verticle");
        //    parameter.catHead.SetActive(true);
        //}
        //else if(parameter.inputHandler.AdjustedJumpDir == JumpInput.Left)
        //{
        //    manager.transform.localScale = new Vector3(1, 1, 1);
        //    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
        //    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
        //    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x - 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
        //    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position+manager.transform.rotation*leftStartPos;
        //    parameter.animator.Play("Jump_Prepare_Horizontal");
        //    parameter.catHead.SetActive(true);
        //}
        //else if (parameter.inputHandler.AdjustedJumpDir == JumpInput.Right)
        //{
        //    manager.transform.localScale = new Vector3(-1, 1, 1);
        //    parameter.catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
        //    parameter.catHead.transform.localScale = new Vector3(40, 40, 40);
        //    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x + 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
        //    parameter.catHead.GetComponent<CatHead>().startPos = manager.transform.position+manager.transform.rotation*rightStartPos;
        //    parameter.animator.Play("Jump_Prepare_Horizontal");
        //    parameter.catHead.SetActive(true);
        //}
    }
}
#endregion