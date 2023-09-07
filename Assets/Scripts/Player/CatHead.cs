using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using UnityEngine;

public class CatHead : MonoBehaviour
{
    [SerializeField] TileManager tileManager;
    [SerializeField] PlayerFSM player;
    public Vector3 collidePos;
    [SerializeField] AnimationCurve flyingCurve;
    public JumpInput jumpDir;
    private bool canFly;
    private float t;
    private float displacement;
    SpriteRenderer headSprite;
    public Vector3 startPos;
    private bool prepareAnimationOver;

    [SerializeField] Transform head_left, head_right, leg_left, leg_right;

    private void Awake()
    {
        headSprite = GetComponent<SpriteRenderer>();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnEnable()
    {
        headSprite.enabled = false;
        jumpDir = player.parameter.inputHandler.jumpDir;
        t = 0;
    }

    private void OnDisable()
    {
        player.parameter.leftShape.SetActive(false);
        player.parameter.rightShape.SetActive(false);
        canFly = false;
        prepareAnimationOver = false;
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
        }

        Fly();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LightTile"))
        {
            //Debug.Log("trigger");
            collidePos = collision.ClosestPoint(transform.position);
            if (tileManager.SearchColor(collidePos))
            {
                canFly = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;   //Close head's collider
                //collidePos = transform.position;
                Debug.DrawRay(collidePos,Vector2.up*5,Color.red,100000);
                Debug.DrawRay(collidePos,Vector2.left*5,Color.red,100000);
                transform.SetParent(collision.transform, true);     //let head maintain its world position!
                StartCoroutine(CatFly());   //Fly cat to the postion of head
            }
        }
    }

    void Fly()
    {
        if(canFly && t <= 10f)      //Manually set the flying duration as 1f in the flyingCurve!
        {
            t += Time.deltaTime;
            displacement = flyingCurve.Evaluate(t);
            if(jumpDir == JumpInput.Up)
            {
                transform.position = new Vector3(startPos.x, startPos.y + displacement, startPos.z);
            }
            else if(jumpDir == JumpInput.Down)
            {
                transform.position = new Vector3(startPos.x, startPos.y - displacement, startPos.z);
            }
            else if(jumpDir==JumpInput.Left)
            {
                transform.position = new Vector3(startPos.x - displacement, startPos.y, startPos.z);
            }
            else
            {
                transform.position = new Vector3(startPos.x + displacement, startPos.y, startPos.z);
            }
        }
        else if(canFly && t > 10f)      //if head collide with nothing, fly back and leave JumpState!
        {
            t = 0;
            //canFly = false;
            player.parameter.leftShape.SetActive(false);
            player.parameter.rightShape.SetActive(false);
            player.parameter.jumpFinished = true;           //this is the value to evaluate whether StateMachine change state
        }
    }

    IEnumerator CatFly()
    {
        t = 0f;
        while(Vector3.Distance(head_left.position, leg_left.position) >= 5f || Vector3.Distance(head_right.position, leg_right.position) >= 5f)
        {
            t += Time.deltaTime;
            displacement = flyingCurve.Evaluate(t);
            switch(jumpDir)
            {
                case JumpInput.Up :
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + displacement, player.transform.position.z);
                    break;
                case JumpInput.Down :
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - displacement, player.transform.position.z);
                    break; 
                case JumpInput.Left :
                    player.transform.position = new Vector3(player.transform.position.x - displacement, player.transform.position.y, player.transform.position.z);
                    break;
                case JumpInput.Right :
                    player.transform.position = new Vector3(player.transform.position.x + displacement, player.transform.position.y, player.transform.position.z);
                    break;
            }
            yield return null;
        }

        headSprite.enabled = false;

        player.parameter.animator.Play("Jump_Rolling");
        yield return null;

        while (player.parameter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        
        //TODO:Play the audio of landing

        player.transform.position = collidePos;
        //TODD: Check if animation is over, set head unactive and change state
        switch (jumpDir)
        {
            case JumpInput.Up :
                player.parameter.platformDir = PlatformDirType.Down;
                player.transform.rotation=Quaternion.Euler(0,0,-180);
                // player.transform.Rotate(0, 0, -180);    //rotate flip
                player.transform.localScale=new(-player.transform.localScale.x,player.transform.localScale.y,player.transform.localScale.z);
                break;
            case JumpInput.Down :
                player.parameter.platformDir = PlatformDirType.Up;
                player.transform.rotation=Quaternion.Euler(0,0,0);
                // player.transform.Rotate(0, 0, -180);    //rotate flip
                player.transform.localScale=new(-player.transform.localScale.x,player.transform.localScale.y,player.transform.localScale.z);
                break;
            case JumpInput.Left :
                player.parameter.platformDir = PlatformDirType.Right;
                player.transform.rotation=Quaternion.Euler(0,0,-90);
                // player.transform.Rotate(0, 0, -90);    //rotate flip
                break;
            case JumpInput.Right :
                player.parameter.platformDir = PlatformDirType.Left;
                player.transform.rotation=Quaternion.Euler(0,0,90);
                // player.transform.Rotate(0, 0, 90);    //rotate flip
                break;
        }

        transform.SetParent(player.transform, true);   //set head's parent back to cat!
        //Debug.Log(transform.parent.name);
        yield return null;
        player.parameter.jumpFinished = true;      //tell machine to exit JumpState!
    }
}
