using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerC : MonoBehaviour
{
    [SerializeField]
    float inputX;
    [SerializeField]
    float inputY;
    [SerializeField]
    bool isAxisRaw = true;
    
    Vector2 _speed;
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float jumpSpeed = 10;
    Vector3 _newPosition;
    Transform transform_;

    float Gravity = -15;
    public bool isGravity = true;
    public float floatingRate = 0;
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
        //Debug.Log(Time.deltaTime);
        setParam();
        input();
        doGravity();
        beforeMove();
        rayCheck();
        Move();
        resetParam();
        //beforeJump();
        //Jump();
    }

    void setParam()
    {
        isFalling = true;
        //RayCheckBox = new Rect(_boxCollider.bounds.min.x,
        //    _boxCollider.bounds.min.y,
        //    _boxCollider.bounds.size.x,
        //    _boxCollider.bounds.size.y);
    }
    void doGravity()
    {
        if (!isGravity) return;

        _speed.y += (Gravity * Time.deltaTime);
        if (floatingRate != 0) 
        {
            _speed.y *= floatingRate;
        }  
    }

    void rayCheck()
    {
        SetRayparam();
        RayCheckFoot();
        
    }
    void SetRayparam()
    {
        RayCheckBox = new Rect(_boxCollider.bounds.min.x,
            _boxCollider.bounds.min.y,
            _boxCollider.bounds.size.x,
            _boxCollider.bounds.size.y);
    }

    void RayCheckFoot()
    {
        if (_newPosition.y<0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;

        }
        if (Gravity>0 && !isFalling)
        {
            return;
        }
        float rayLength = RayCheckBox.height / 2 + RayOffset;
        if (_newPosition.y < 0)
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }
        Vector2 RayCenter = RayCheckBox.center;
        RayCenter.y += RayOffset;
        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        hitInfo[0] = RayCast(RayCenter, -Vector2.up, rayLength, PlatformMask, Color.red, true);
        if (hitInfo[0])
        {
            StandingOn = hitInfo[0].collider.gameObject;
            isFalling = false;
            isCollDown = true;
            if (helpMoveParam.y > 0)
            {
                _newPosition.y = _speed.y * Time.deltaTime;
                isCollDown = false;
            }
            else
            {
                _newPosition.y = -Mathf.Abs(hitInfo[0].point.y - RayCenter.y) +
                    RayCheckBox.height / 2 + RayOffset;
            }
            if (Mathf.Abs(_newPosition.y) < _smallValue)
            {
                _newPosition.y = 0;
            }
            else
            {
                isCollDown = false;

            }
        }
    }

    RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, 
        LayerMask mask, Color color, bool drawGizmos = false)
    {
        if (drawGizmos)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint,rayDirection,rayDistance,mask);
    }
    void resetParam()
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


    private void LateUpdate()
    {
        //
    }
    private void FixedUpdate()
    {
        //Debug.LogError(Time.deltaTime);
    }

    void input()
    {
        //if (input.getkeydown(keycode.a))
        //{
        //    debug.log("a pressed");
        //}
        //if (input.getkeyup(keycode.a))
        //{
        //    debug.log("a released");
        //}
        if (isAxisRaw) 
        {
            inputX = Input.GetAxisRaw("Horizontal");
            //inputY = Input.GetAxisRaw("Jump");
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");  //有惯性
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

 
      
        //Debug.Log(inputX);
    }
    void beforeMove()
    {
        _speed.x = inputX * moveSpeed;
        _newPosition = _speed * Time.deltaTime;
    }
    void Move() 
    {
        transform_.Translate(_newPosition,Space.World);
    }
    void beforeJump()
    {
        _speed.y = inputY * jumpSpeed;
        _newPosition = _speed * Time.deltaTime;
    }
    void Jump()
    {
        transform_.Translate(_newPosition, Space.World);
    }
}
