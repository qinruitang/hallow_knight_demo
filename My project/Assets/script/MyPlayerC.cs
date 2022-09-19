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

    // Start is called before the first frame update
    void Start()
    {
        transform_ = this.transform;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.deltaTime);
        input();
        beforeMove();
        Move();
        //beforeJump();
        //Jump();
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
