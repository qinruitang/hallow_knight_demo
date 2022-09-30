using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerC : MonoBehaviour
{
    public float inputX, inputY;
    public float moveSpeed;
    Vector2 _speed;
    Vector3 _newPosition;
    Transform transform_;
    public bool isAxisRaw = true; 
    float Gravity = -15;
    public bool isGravity = true;
    public float floatingRate = 0; //漂浮参数
    bool isRight;

    public BoxCollider2D _boxCollider;
    Rect RayCheckBox;


    bool isFalling = false;
    float RayOffset = 0.15f;
    public LayerMask PlatformMask = 0;
    GameObject StandingOn; 
    bool isCollDown;
    Vector2 helpMoveParam;
    float _smallValue = 0.0001f; 

    bool IsGrounded { get { return isCollDown; } }
    public bool canJump = true;
    float JumpPower = 2;



    // Start is called before the first frame update
    void Start()
    {
        transform_ = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        SetParam();
        input();
        DoGravity();
        beforeMove();
        RayCheck();
        move();
        ResetParam();
    }

    void input()
    {
        if (isAxisRaw)
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }
        if (Input.GetButtonDown("Jump"))
        {
            JumpStart();
        }
        if (Input.GetMouseButton(1))
        {
            floatingRate = 0.75f;
        }
        else
        {
            floatingRate = 0;
        }

    }
    void beforeMove()
    {
        _speed.x = inputX * moveSpeed;
        _newPosition = _speed * Time.deltaTime;
        if (inputX < -0.1f && isRight)
        {
            flipModel();
        }
        else if (inputX > 0.1f && !isRight)
        {
            flipModel();
        }
    }
    void flipModel()
    {
        isRight = !isRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    void move()
    {
        transform_.Translate(_newPosition, Space.World);
    }
    void DoGravity()
    {
        if (!isGravity) return;

        _speed.y += (Gravity * Time.deltaTime);
        if (floatingRate != 0)
        {
            _speed.y *= floatingRate; // 几何级衰减
        }
    }

    void RayCheck()
    {
        SetRayParam();
        RayCheckFoot();
    }
    void SetRayParam()
    {
        RayCheckBox = new Rect(_boxCollider.bounds.min.x,
                                 _boxCollider.bounds.min.y,
                                 _boxCollider.bounds.size.x,
                                 _boxCollider.bounds.size.y
                                );
    }


  
    void RayCheckFoot()
    {
        if (_newPosition.y < 0)  // 如果计算后的新Y<0, 说明角色在下降.
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        
        if ((Gravity > 0) && (!isFalling)) // 如果DoGravity不是向下, 或者状态不是下降,则不用检测脚下. 比如上升过程
        {
            return;
        }
        float rayLength = RayCheckBox.height / 2 + RayOffset; //检测射线从中心发, 所以高度一半 + rayoffset就是检测长度.
        if (_newPosition.y < 0) // 如果是下降,  检测长度加上当前绝对值下降值. 如图
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }
        Vector2 RayCenter = RayCheckBox.center;
        RayCenter.y += RayOffset;

        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        
        //if (_newPosition.y > 0) // 主要区别是否碰到了可穿越平台检测
        //{
        //    hitInfo[0] = RayCast(RayCenter, -(Vector2.up), rayLength, PlatformMask  , Color.blue, true);
        //}
        //else
        {
            hitInfo[0] = RayCast(RayCenter, -(Vector2.up), rayLength, PlatformMask, Color.blue, true);
        }
        if (hitInfo[0]) //碰撞了
        { 
            StandingOn = hitInfo[0].collider.gameObject;   // 记录要停留的物体
            isFalling = false;
            isCollDown = true; 
            // 跳跃的情况   或者向上冲的技能, 这时候 不能检测.
            if (helpMoveParam.y > 0) 
            {
                _newPosition.y = _speed.y * Time.deltaTime;
                isCollDown = false;
            }
            else //正常下坠情况
            {
                _newPosition.y = -Mathf.Abs(hitInfo[0].point.y - RayCenter.y)
              + RayCheckBox.height / 2
              + RayOffset;
                
               
            }
            if (Mathf.Abs(_newPosition.y) < _smallValue) // 消除float类型的误差.如果小于临界值就当做0处理
            {
                _newPosition.y = 0;
            }
        }
        else
        {
            isCollDown = false;
        } 
    }
    public RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }
    void SetParam()
    { 
        isFalling = true;
    }

    void ResetParam()
    {
        if (Time.deltaTime > 0)
        {
            _speed = _newPosition / Time.deltaTime;
        }
        helpMoveParam.x = 0;
        helpMoveParam.y = 0; 
    }
   
    void JumpStart()
    {
        if (IsGrounded)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
        if (!canJump)
        {
            return;
        }
        _speed.y = Mathf.Sqrt(2f * JumpPower * Mathf.Abs(Gravity));
        helpMoveParam.y = _speed.y;

    }
}
