using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraC : MonoBehaviour
{
    public bool isFollow = true;
    float _offsetz;
    public Vector3 CameraOffset;
    public float HlookForwardDistance = 3;
    public float lookForwardTriggerDistance = 0.1f;
    public float resetSpeed = 2;
    public float followSpeed = 5;
    public Transform _target;
    public bool lookForward = false;
    Vector3 _lastTargetPosition, _currentVelocity, _lookAheadPos;

    // Start is called before the first frame update
    void Start()
    {
        _lastTargetPosition = _target.position;
        _offsetz = (transform.position.z-_target.position.z);

    }

    // Update is called once per frame


    void Update()
    {
        //transform.position = _target.position + Vector3.forward * _offsetz + CameraOffset;
        if (!isFollow)
        {
            return;
        }
        if (lookForward)
        {
            float xMoveDelta = (_target.position - _lastTargetPosition).x;
            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookForwardTriggerDistance;
            if (updateLookAheadTarget)
            {
                _lookAheadPos = HlookForwardDistance * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                _lookAheadPos = Vector3.MoveTowards(_lookAheadPos, Vector3.zero, Time.deltaTime * resetSpeed);
            }
            Vector3 aheadtargetPos = _target.position + _lookAheadPos + Vector3.forward * _offsetz + CameraOffset;
                Vector3 newCameraPostion = Vector3.Lerp(transform.position,aheadtargetPos,Time.deltaTime*followSpeed);
                transform.position = newCameraPostion;
                _lastTargetPosition = _target.position;
        }
    }

}
