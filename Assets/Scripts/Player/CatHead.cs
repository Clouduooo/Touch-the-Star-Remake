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

    private void Awake()
    {
        headSprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        headSprite.enabled = false;
        jumpDir = player.parameter.inputHandler.jumpDir;
        //startPos = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y + 16.5f, player.gameObject.transform.position.z);
        t = 0;
    }

    private void OnDisable()
    {
        canFly = false;
    }

    private void Update()
    {
        if(player.parameter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f)
        {
            canFly = true;
            headSprite.enabled = true;
            player.parameter.leftShape.SetActive(true);
            player.parameter.rightShape.SetActive(true);
        }
        Fly();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LightTile"))
        {
            collidePos = transform.position;
            if (tileManager.SearchColor(collidePos))
            {
                canFly = false;
                collidePos = transform.position;
                transform.SetParent(collision.transform, false);
                Debug.Log("Enter Collider");
                StartCoroutine(CatFly());   //Fly cat to the postion of head
            }
        }
    }

    void Fly()
    {
        if(canFly && t <= 1f)      //Manually set the flying duration as 1f in the flyingCurve!
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
        else if(canFly && t > 1f)
        {
            Debug.Log("into!");
            t = 0;
            canFly = false;
            player.parameter.leftShape.SetActive(false);
            player.parameter.rightShape.SetActive(false);
            player.parameter.jumpFinished = true;
        }
    }

    IEnumerator CatFly()
    {
        t = 0f;
        while(Vector3.Distance(player.transform.position, collidePos) >= 2f)
        {
            t += Time.deltaTime;
            displacement = flyingCurve.Evaluate(t);
            switch(jumpDir)
            {
                case JumpInput.Up :
                    player.transform.position = new Vector3(startPos.x, startPos.y + displacement, startPos.z);
                    break;
                case JumpInput.Down :
                    player.transform.position = new Vector3(startPos.x, startPos.y - displacement, startPos.z);
                    break; 
                case JumpInput.Left :
                    player.transform.position = new Vector3(startPos.x - displacement, startPos.y, startPos.z);
                    break;
                case JumpInput.Right :
                    player.transform.position = new Vector3(startPos.x + displacement, startPos.y, startPos.z);
                    break;
            }
            yield return null;
        }
        player.parameter.animator.Play("Jump_Rolling");
        //TODD: Check if animation is over, set head unactive and change state
        switch (jumpDir)
        {
            case JumpInput.Up :
                player.parameter.platformDir = PlatformDirType.Down; break;
            case JumpInput.Down :
                player.parameter.platformDir = PlatformDirType.Up; break;
            case JumpInput.Left :
                player.parameter.platformDir = PlatformDirType.Right; break;
            case JumpInput.Right :
                player.parameter.platformDir = PlatformDirType.Left; break;
        }
        player.transform.Rotate(0, 0, -180);    //rotate flip
        //transform.SetParent(player.transform, false);   //set head's parent back to cat! 
    }
}
