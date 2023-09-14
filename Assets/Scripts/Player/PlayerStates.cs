using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;

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

    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
        if(parameter.inputHandler.AdjustedJumpDir != JumpInput.None)
        {
            manager.TransitionState(StateType.JumpNew);
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
            manager.TransitionState(StateType.JumpNew);
        }
        else if(parameter.inputHandler.AdjustedMovementDir.x == 0)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    public void OnFixedUpdate()
    {
        
    }

    void DetectMove()
    {
        if(parameter.inputHandler.AdjustedMovementDir.x > 0 && parameter.direction!=-1)
        {
            parameter.direction = 1;
        }
        else if(parameter.inputHandler.AdjustedMovementDir.x < 0 && parameter.direction!=1)
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
            parameter.isWalking = true;
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

    public void OnFixedUpdate()
    {
        
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
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, manager.transform.up, Mathf.Infinity, parameter.lightLayer);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hit.point+0.5f*manager.transform.up))
                {
                    parameter.jumpHit = hit;
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
            }
            case JumpInput.Left:
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                Debug.DrawRay(hit.point,10*Vector2.up,Color.red,100000);
                Debug.DrawRay(hit.point,10*Vector2.right,Color.red,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.up,Color.blue,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.right,Color.blue,100000);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hit.point+0.5f*-manager.transform.right))
                {
                    parameter.jumpHit = hit;
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
            }

            case JumpInput.Right:
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                Debug.DrawRay(hit.point,10*Vector2.up,Color.red,100000);
                Debug.DrawRay(hit.point,10*Vector2.right,Color.red,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.up,Color.blue,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.right,Color.blue,100000);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hit.point+0.5f*manager.transform.right))
                {
                    parameter.jumpHit = hit;
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

#region JumpStateNew
public class JumpStateNew : IPlayerState
{
    readonly private PlayerFSM manager;
    readonly private PlayerParameter parameter;

    readonly private Vector3 vertStartPos=new(0,16.5f,0);
    readonly private Vector3 rightStartPos=new(12.6f,6.2f,0);
    readonly private Vector3 leftStartPos=new(-12.6f,6.2f,0);

    private JumpInput jumpInput,adjustedJumpInput;
    private Vector2 jumpStartPos;
    readonly private GameObject catHead;
    private Vector2 hitPos;
    private float totalDistance;
    private readonly Rigidbody2D catHeadRb;
    private readonly Rigidbody2D catLegRb;
    private Vector2 headToLeg;
    private bool isCloseJump;

    private enum JumpSubState
    {
        JumpPrepare,
        PreAnim,
        HeadFly,
        LegFly,
        RollingAnim,
        JumpEnd,
    }
    private JumpSubState jumpSubState;

    public JumpStateNew(PlayerFSM manager)  //Get reference of PlayerFSM and Player Data from Initiate Function
    {
        this.manager = manager;
        parameter = manager.parameter;
        catHead=parameter.catHead;
        catHeadRb=catHead.GetComponent<Rigidbody2D>();
        catLegRb=manager.GetComponent<Rigidbody2D>();
    }

    public void OnEnter()
    {
        isCloseJump = false;
        jumpInput=parameter.inputHandler.jumpDir;
        adjustedJumpInput=parameter.inputHandler.AdjustedJumpDir;
        jumpSubState=JumpSubState.JumpPrepare;
        parameter.inJumpState=true;
        catLegRb.velocity=Vector2.zero;
    }

    public void OnExit()
    {
        catHead.SetActive(false);
        parameter.inJumpState=false;
    }

    public void OnUpdate()
    {
        if(jumpSubState==JumpSubState.JumpPrepare)
        {
            parameter.isJumpPreAnimFin=false;
            JumpPrepare();
        }
        else if(jumpSubState==JumpSubState.PreAnim)
        {
            if(parameter.isJumpPreAnimFin)
                jumpSubState=JumpSubState.HeadFly;
        }
        else if(jumpSubState==JumpSubState.RollingAnim)
        {
            if (parameter.isJumpRollingAnimFin)
            {
                //get object
                parameter.extendDuration = Vector2.Distance(hitPos, jumpStartPos) / parameter.jumpDurtaionFix;
                parameter.isWalking = false;
                parameter.circleObjectPool.C_Pool.Get();
                jumpSubState = JumpSubState.JumpEnd;
            }
        }
        if (jumpSubState==JumpSubState.JumpEnd)
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
    

    public void OnFixedUpdate()
    {
        if(jumpSubState==JumpSubState.HeadFly)
        {
            HeadFly();
        }
        else if(jumpSubState==JumpSubState.LegFly)
        {
            LegFly();
        }
    }

    void JumpPrepare()
    {
        if (adjustedJumpInput == JumpInput.Down)
        {
            jumpSubState = JumpSubState.JumpEnd;      //change state
            return;
        }

        switch (adjustedJumpInput)
        {
            case JumpInput.Up:
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, manager.transform.up, Mathf.Infinity, parameter.lightLayer);
                hitPos=hit.point;
                Debug.DrawRay(hit.point,10*Vector2.up,Color.red,100000);
                Debug.DrawRay(hit.point,10*Vector2.right,Color.red,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.up,Color.blue,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.right,Color.blue,100000);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hitPos+0.5f*manager.transform.up))
                {
                    catHead.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    catHead.transform.localScale = new Vector3(40, 40, 40);
                    jumpStartPos = manager.transform.position + manager.transform.rotation * vertStartPos;
                    parameter.animator.Play("Jump_Prepare_Verticle");
                }
                else
                {
                    jumpSubState = JumpSubState.JumpEnd;
                    return;
                }
                break;
            }
            case JumpInput.Left:
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, -manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                hitPos=hit.point;
                Debug.DrawRay(hit.point,10*Vector2.up,Color.red,100000);
                Debug.DrawRay(hit.point,10*Vector2.right,Color.red,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.up,Color.blue,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.right,Color.blue,100000);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hitPos+0.5f*-manager.transform.right))
                {
                    manager.transform.localScale = new Vector3(1, 1, 1);
                    catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    catHead.transform.localScale = new Vector3(40, 40, 40);
                    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x - 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
                    jumpStartPos = manager.transform.position + manager.transform.rotation * leftStartPos;
                    parameter.animator.Play("Jump_Prepare_Horizontal");
                }
                else
                {
                    jumpSubState = JumpSubState.JumpEnd;
                    return;
                }
                break;
            }

            case JumpInput.Right:
            {
                RaycastHit2D hit=Physics2D.Raycast((Vector2)parameter.jumpCheckRayStartPoint.transform.position, manager.transform.right, Mathf.Infinity, parameter.lightLayer);
                hitPos=hit.point;
                Debug.DrawRay(hit.point,10*Vector2.up,Color.red,100000);
                Debug.DrawRay(hit.point,10*Vector2.right,Color.red,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.up,Color.blue,100000);
                Debug.DrawRay((Vector3)hit.point+0.5f*-manager.transform.right,10*Vector2.right,Color.blue,100000);
                if(!hit.collider.IsUnityNull() && parameter.tileManager.SearchColor((Vector3)hitPos+0.5f*manager.transform.right))
                {
                    manager.transform.localScale = new Vector3(-1, 1, 1);
                    catHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    catHead.transform.localScale = new Vector3(40, 40, 40);
                    // parameter.catHead.GetComponent<CatHead>().startPos = new Vector3(manager.transform.position.x + 12.6f, manager.transform.position.y + 6.2f, manager.transform.position.z);
                    jumpStartPos = manager.transform.position + manager.transform.rotation * rightStartPos;                            
                    parameter.animator.Play("Jump_Prepare_Horizontal");
                }
                else
                {
                    jumpSubState = JumpSubState.JumpEnd;
                    return;
                }
                break;
            }
        }
        totalDistance = Vector2.Distance(hitPos,jumpStartPos);
        //parameter.cam.totalDistance = totalDistance;    //give value to Camera Script
        catHead.transform.position=jumpStartPos;
        headToLeg=manager.transform.position-catHead.transform.position;
        jumpSubState=JumpSubState.PreAnim;
    }

    void HeadFly()
    {
        catHead.SetActive(true);
        parameter.body.SetActive(true);

        if(Vector2.Distance(jumpStartPos, hitPos) > Vector2.Distance((Vector2)manager.transform.position, hitPos) ||
           Vector2.Distance((Vector2)manager.transform.position, hitPos) - Vector2.Distance(jumpStartPos, hitPos) <= 12f)
        {
            //Debug.Log("Skip");
            isCloseJump = true;           //skip the process of giving velocity to cat's head and leg
            catHeadRb.velocity = Vector2.zero;
            catHead.transform.position = hitPos;
            jumpSubState = JumpSubState.LegFly;
        }
        //if(Vector2.Distance((Vector2)catHead.transform.position, hitPos) >= 2f)
        {
            float lerpRatio = Vector2.Distance((Vector2)catHead.transform.position, jumpStartPos) / totalDistance;
            float flySpeed = parameter.flyingCurve.Evaluate(lerpRatio+0.01f) * parameter.flySpeedFix;
            catHeadRb.velocity = jumpInput switch
            {
                JumpInput.Up => new Vector2(0, flySpeed),
                JumpInput.Down => new Vector2(0, -flySpeed),
                JumpInput.Left => new Vector2(-flySpeed, 0),
                JumpInput.Right => new Vector2(flySpeed, 0),
                _ => Vector2.zero,
            };
            if(flySpeed*Time.fixedDeltaTime<Vector2.Distance((Vector2)catHead.transform.position, hitPos))
                return;
        }
        //parameter.cam.totalDistance = totalDistance;    //give value to Camera Script
        catHeadRb.velocity=Vector2.zero;
        catHead.transform.position=hitPos;
        jumpSubState=JumpSubState.LegFly;
    }

    void LegFly()
    {
        if(!isCloseJump && Vector2.Distance((Vector2)manager.transform.position, hitPos+headToLeg) >= 3f)
        {
            float lerpRatio = Vector2.Distance((Vector2)manager.transform.position, jumpStartPos+headToLeg) / totalDistance;
            float flySpeed = parameter.flyingCurve.Evaluate(lerpRatio+0.01f) * parameter.flySpeedFix;
            catLegRb.velocity = jumpInput switch
            {
                JumpInput.Up => new Vector2(0, flySpeed),
                JumpInput.Down => new Vector2(0, -flySpeed),
                JumpInput.Left => new Vector2(-flySpeed, 0),
                JumpInput.Right => new Vector2(flySpeed, 0),
                _ => Vector2.zero,
            };
            if(flySpeed*Time.fixedDeltaTime<Vector2.Distance((Vector2)manager.transform.position, hitPos+headToLeg))
                return;
        }

        catHead.SetActive(false);
        parameter.body.SetActive(false);

        switch (jumpInput)
        {
            case JumpInput.Up:
                manager.parameter.platformDir = PlatformDirType.Down;
                manager.transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case JumpInput.Down:
                manager.parameter.platformDir = PlatformDirType.Up;
                manager.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case JumpInput.Left:
                manager.parameter.platformDir = PlatformDirType.Right;
                manager.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case JumpInput.Right:
                manager.parameter.platformDir = PlatformDirType.Left;
                manager.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        catLegRb.velocity=Vector2.zero;
        manager.transform.position = hitPos;

        parameter.isJumpRollingAnimFin=false;
        manager.parameter.animator.Play("Jump_Rolling");
        
        jumpSubState=JumpSubState.RollingAnim;
        
        //TODO:Play the audio of landing
    }
}
#endregion