using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using UnityEngine;

public class CatHead : MonoBehaviour
{
    [SerializeField] TileManager tileManager;
    [SerializeField] PlayerFSM player;
    [SerializeField] AnimationCurve flyingCurve;
    public JumpInput jumpDir;
    private bool canFly;
    SpriteRenderer headSprite;
    public Vector3 startPos;
    private bool prepareAnimationOver;
    public float lerpRatio;
    public float totalDistance;
    public float flySpeed;
    public Vector2 totalSpeed;
    private Rigidbody2D rb;
    private bool headFinished;
    [SerializeField] float initFlySpeed;    //change in inspector
    
    [SerializeField] Transform head_left, head_right, leg_left, leg_right;

    private void Awake()
    {
        headSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnEnable()
    {
        headFinished = true;
        headSprite.enabled = false;
        jumpDir = player.parameter.inputHandler.jumpDir;
        totalDistance = Vector2.Distance(player.parameter.jumpHit.point, (Vector2)startPos);
        flySpeed = 0;
    }

    private void OnDisable()
    {
        //player.parameter.leftShape.SetActive(false);
        //player.parameter.rightShape.SetActive(false);
        canFly = false;
        headFinished = false;
        prepareAnimationOver = false;
    }

    private void FixedUpdate()
    {
        AddFlySpeed();
    }

    private void AddFlySpeed()
    {
        rb.velocity = totalSpeed;
    }

    private void Update()
    {
        // if prepareAnimation hasn't over, into this if
        if (!prepareAnimationOver && player.parameter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            canFly = true;
            headSprite.enabled = true;
            player.parameter.leftShape.SetActive(true);
            player.parameter.rightShape.SetActive(true);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            prepareAnimationOver = true;   //set opposite, make sure this "if" can only in at a time
            flySpeed = initFlySpeed;
        }
        Fly();
    }

    void Fly()
    {
        if(canFly && Vector2.Distance((Vector2)startPos, player.parameter.jumpHit.point) >= 2f)
        {
            lerpRatio = Vector2.Distance((Vector2)transform.position, startPos) / totalDistance;
            flySpeed = flyingCurve.Evaluate(lerpRatio);
            if(jumpDir == JumpInput.Up)
            {
                totalSpeed = new Vector2(0, flySpeed);
            }
            else if(jumpDir == JumpInput.Down)
            {
                totalSpeed = new Vector2(0, -flySpeed);
            }
            else if(jumpDir==JumpInput.Left)
            {
                totalSpeed = new Vector2(-flySpeed, 0);
            }
            else
            {
                totalSpeed = new Vector2(flySpeed, 0);
            }
        }
        else if(canFly && headFinished)     //might have some bugs, if so, use Enummerator to held Fly() function
        {
            //StartCoroutine(CatFly());
        }
    }

    IEnumerator CatFly()
    {
        while(Vector2.Distance((Vector2)player.transform.position, player.parameter.jumpHit.point) >= 3f)
        {
            lerpRatio = Vector2.Distance((Vector2)player.transform.position, startPos) / totalDistance;
            flySpeed = flyingCurve.Evaluate(lerpRatio);
            switch(jumpDir)
            {
                case JumpInput.Up :
                    totalSpeed = new Vector2(0, flySpeed);
                    break;
                case JumpInput.Down :
                    totalSpeed = new Vector2(0, -flySpeed);
                    break; 
                case JumpInput.Left :
                    totalSpeed = new Vector2(-flySpeed, 0);
                    break;
                case JumpInput.Right :
                    totalSpeed = new Vector2(flySpeed, 0);
                    break;
            }
            yield return null;
        }

        headSprite.enabled = false;

        player.parameter.leftShape.SetActive(false);
        player.parameter.rightShape.SetActive(false);

        switch (jumpDir)
        {
            case JumpInput.Up:
                player.parameter.platformDir = PlatformDirType.Down;
                player.transform.rotation = Quaternion.Euler(0, 0, -180);
                // player.transform.Rotate(0, 0, -180);    //rotate flip
                player.transform.localScale = new(-player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);
                break;
            case JumpInput.Down:
                player.parameter.platformDir = PlatformDirType.Up;
                player.transform.rotation = Quaternion.Euler(0, 0, 0);
                // player.transform.Rotate(0, 0, -180);    //rotate flip
                player.transform.localScale = new(-player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);
                break;
            case JumpInput.Left:
                player.parameter.platformDir = PlatformDirType.Right;
                player.transform.rotation = Quaternion.Euler(0, 0, -90);
                // player.transform.Rotate(0, 0, -90);    //rotate flip
                break;
            case JumpInput.Right:
                player.parameter.platformDir = PlatformDirType.Left;
                player.transform.rotation = Quaternion.Euler(0, 0, 90);
                // player.transform.Rotate(0, 0, 90);    //rotate flip
                break;
        }

        player.transform.position = player.parameter.jumpHit.point;

        player.parameter.animator.Play("Jump_Rolling");
        yield return null;

        while (player.parameter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        
        //TODO:Play the audio of landing


        yield return null;
        player.parameter.jumpFinished = true;      //tell machine to exit JumpState!
    }
}
