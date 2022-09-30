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
    public float floatingRate = 0; //Ư������
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
            _speed.y *= floatingRate; // ���μ�˥��
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
        if (_newPosition.y < 0)  // �����������Y<0, ˵����ɫ���½�.
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        
        if ((Gravity > 0) && (!isFalling)) // ���DoGravity��������, ����״̬�����½�,���ü�����. ������������
        {
            return;
        }
        float rayLength = RayCheckBox.height / 2 + RayOffset; //������ߴ����ķ�, ���Ը߶�һ�� + rayoffset���Ǽ�ⳤ��.
        if (_newPosition.y < 0) // ������½�,  ��ⳤ�ȼ��ϵ�ǰ����ֵ�½�ֵ. ��ͼ
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }
        Vector2 RayCenter = RayCheckBox.center;
        RayCenter.y += RayOffset;

        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        
        //if (_newPosition.y > 0) // ��Ҫ�����Ƿ������˿ɴ�Խƽ̨���
        //{
        //    hitInfo[0] = RayCast(RayCenter, -(Vector2.up), rayLength, PlatformMask  , Color.blue, true);
        //}
        //else
        {
            hitInfo[0] = RayCast(RayCenter, -(Vector2.up), rayLength, PlatformMask, Color.blue, true);
        }
        if (hitInfo[0]) //��ײ��
        { 
            StandingOn = hitInfo[0].collider.gameObject;   // ��¼Ҫͣ��������
            isFalling = false;
            isCollDown = true; 
            // ��Ծ�����   �������ϳ�ļ���, ��ʱ�� ���ܼ��.
            if (helpMoveParam.y > 0) 
            {
                _newPosition.y = _speed.y * Time.deltaTime;
                isCollDown = false;
            }
            else //������׹���
            {
                _newPosition.y = -Mathf.Abs(hitInfo[0].point.y - RayCenter.y)
              + RayCheckBox.height / 2
              + RayOffset;
                
               
            }
            if (Mathf.Abs(_newPosition.y) < _smallValue) // ����float���͵����.���С���ٽ�ֵ�͵���0����
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
