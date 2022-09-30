using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerC1 : MonoBehaviour
{
    [SerializeField]
     float inputX;
    [SerializeField]
    bool isAxisRaw = true;

    Vector2 _speed;
    [SerializeField]
    float moveSpeed = 0;

    Vector3 _newPosition;
    Transform tranform_;

    float Gravity = -15;
    public bool isGravity = true;
    public float floatingRate = 0;
    bool isRight;
    public BoxCollider2D _boxCollider;
    Rect RayCheckBox;
    bool isFalling = false;
    float RayOffet = 0.15f;
    public LayerMask PlatformMask = 0;
    GameObject StandingOn;
    bool isCollDown;
    Vector2 helpMoveParam;
    float _smallValue = 0.0001f;
    bool IsGrounded { get { return isCollDown; } }
    public bool canJump =true;
    float JumpPower = 2;

    public int maxJumpNum = 3;
    int leftJumpNum;
    bool isGroundLastFrame;
    bool isJustGround;

    public float inputY;
    float ingoreCrossCheckTime = 2f;
    public LayerMask CrossPlatformMask;
    LayerMask platformMaskSave;
    float headRayCheckLength = 0.15f;
    float downInitOffsetY = 0.1f;
    // Ö¡
    // Start is called before the first frame update
    void Start()
    {
        tranform_ = this.transform;
        leftJumpNum = maxJumpNum;
        platformMaskSave = PlatformMask;
    }

    // Update is called once per frame
    void Update()
    {
        setParam();
        input();
        doGravity();
        beforeMove();
        rayCheck();
        move();
        resetParam(); 
    }
    void setParam()
    {
        isFalling = true;
        isGroundLastFrame = isCollDown;
        isJustGround = false;

        //RayCheckBox = new Rect(_boxCollider.bounds.min.x,
        //    _boxCollider.bounds.min.y,
        //    _boxCollider.bounds.size.x,
        //    _boxCollider.bounds.size.y
        //    );
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
        SetRayParam();
        RayCheckFoot();
        RayCheckHead();
    }
    void RayCheckHead()
    {
        if (_newPosition .y < 0)
        {
            return;
        }
        float rayLength = headRayCheckLength;
        rayLength += RayCheckBox.height / 2;
        Vector2 rayCenter = RayCheckBox.center;
       // rayCenter.y += RayOffet;
        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        hitInfo[0] = RayCast(rayCenter,Vector2.up,rayLength, PlatformMask & ~CrossPlatformMask,Color.green,true);
        if (hitInfo[0])
        {
            _speed.y = 0;
            _newPosition.y = hitInfo[0].distance - RayCheckBox.height / 2;
        }
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
        if (_newPosition.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        if (Gravity > 0 && !isFalling )
        {
            return;
        }
        float rayLength = RayCheckBox.height / 2 + RayOffet;
        if (_newPosition.y < 0)
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }
        Vector2 RayCenter = RayCheckBox.center;
        RayCenter.y += RayOffet;
        RaycastHit2D[] hitInfo = new RaycastHit2D[1];

        if (_newPosition.y > 0)
        {
            hitInfo[0] = RayCast(RayCenter, -Vector2.up, rayLength, PlatformMask & ~CrossPlatformMask, Color.red, true);
        }
        else
        {
            hitInfo[0] = RayCast(RayCenter, -Vector2.up, rayLength, PlatformMask, Color.red, true);
        }
        if (hitInfo[0])
        {
            StandingOn = hitInfo[0].collider.gameObject;
            // ¼ì²â¿É´©Ô½Æ½Ì¨µÄÅÐ¶¨.
            if (!isGroundLastFrame && (hitInfo[0].distance < RayCheckBox.size.y / 2) 
                && (StandingOn.layer == LayerMask.NameToLayer("不可穿越平台")))
            {
                isCollDown = false;
                return;
            } 
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
                    RayCheckBox.height / 2 + RayOffet;
            }
            if (Mathf.Abs(_newPosition.y) < _smallValue)
            {
                _newPosition.y = 0;
            }
        }
        else
        {
            isCollDown = false;
        }
    }

      RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance,
        LayerMask mask ,Color color, bool drawGizmos= false )
    {
        if (drawGizmos)
        {
            Debug.DrawRay(rayOriginPoint,rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance,mask);
    }

    void resetParam()
    { 
        if (Time.deltaTime > 0)
        {
            _speed = _newPosition / Time.deltaTime;
        }
        helpMoveParam.x = 0;
        helpMoveParam.y = 0;
        if (!isGroundLastFrame && isCollDown)
        {
            isJustGround = true;
        }
        if (isJustGround)
        {
            leftJumpNum = maxJumpNum;
        } 
    }
    void JumpStart()
    {
        if (IsGrounded || leftJumpNum > 0)
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
        if (inputY < 0 && IsGrounded)
        {
            if (StandingOn.layer == LayerMask.NameToLayer("可穿越平台"))
            {
                tranform_.position = new Vector2(tranform_.position.x,tranform_.position.y - downInitOffsetY);
                StartCoroutine(closeCrossCheck(ingoreCrossCheckTime));
                return;
            }
        }
        _speed.y = Mathf.Sqrt(2f * JumpPower * Mathf.Abs(Gravity));
        helpMoveParam.y = _speed.y;
        --leftJumpNum;
    }
    IEnumerator closeCrossCheck(float time)
    {
        PlatformMask -= CrossPlatformMask;
        yield return new WaitForSeconds(time);
        PlatformMask = platformMaskSave;
    }

    void input()
    {
        //if( Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.Log("°´ÏÂ¼üÅÌA");
        //}
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    Debug.Log("Ì§Æð¼üÅÌA");
        //}
        if (isAxisRaw)
        {
            inputY = Input.GetAxisRaw("Vertical");
            inputX = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            inputY = Input.GetAxis("Vertical");
            inputX = Input.GetAxis("Horizontal");
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
    void move()
    {
        tranform_.Translate(_newPosition,Space.World);
    }

}
