using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    private bool canMove;
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    private Rigidbody2D rb;
    float speed;            //带方向的speed，综合玩家左右朝向和速度曲线
    float moveTime;
    private float direction;
    public float Direction { get { return direction; } }
    [SerializeField] AnimationCurve moveCurve;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        FaceDirection();
    }

    void Move()
    {
        direction = Input.GetAxisRaw("Horizontal");
        if (canMove)
        {
            //地面上横向移动逻辑
            if (direction != 0 && moveTime < 0.6f)
            {
                speed = (moveCurve.Evaluate(moveTime)) * direction * 2;
                moveTime += Time.deltaTime;
            }
            else if(direction == 0)
            {
                moveTime = 0;
                speed = 0;
            }
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            moveTime = 0;
            speed = 0;
            rb.velocity = new Vector2(speed, 0);
        }
    }

    void FaceDirection()
    {
        if(direction != 0)
        {
            //gameObject.transform.localScale = new Vector3(direction* gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            gameObject.transform.localScale = new Vector3(-direction, 1, 1);
        }
    }
}
