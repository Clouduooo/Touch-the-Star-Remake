using System.Collections;
using System.Collections.Generic;
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
    Vector3 startPos;

    private void OnEnable()
    {
        startPos = new Vector3(player.gameObject.transform.position.x + 1.5f, player.gameObject.transform.position.y + 15f, player.gameObject.transform.position.z);
        t = 0;
    }

    private void OnDisable()
    {
        canFly = false;
    }

    private void Update()
    {
        if(player.parameter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            jumpDir = player.parameter.inputHandler.jumpDir;
            canFly = true;
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
                //player.parameter.animator.Play("Jump_Rolling");
                collidePos = transform.position;
                transform.SetParent(collision.transform, false);
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
        else
        {
            t = 0;
            canFly = false;
            player.parameter.jumpFinished = true;
        }
    }
}
