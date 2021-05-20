using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class cameraAim : MonoBehaviour
{
    [SerializeField] Transform targetToLook = default;
    [SerializeField] Vector3 offset = default;
    //[SerializeField, Range(0.0f, 5.0f)] float smoothTime = 1.0f;
    Transform _tran;
    Quaternion origingalRot = default;

    Vector3 velo = Vector3.zero;
    private void Awake()
    {
        _tran = transform;
        origingalRot = _tran.rotation;
        if (offset == Vector3.zero)
            offset = _tran.position - targetToLook.position;
    }

    private void LateUpdate()
    {
        //_tran.position = Vector3.SmoothDamp(_tran.position, targetToLook.position + offset, ref velo, smoothTime);
        _tran.rotation = Quaternion.LookRotation(targetToLook.position-_tran.position, Vector3.up);
    }

    public void ResetRot()
    {
        _tran.DORotateQuaternion(origingalRot, 1.0f);
    }

    /*private void LateUpdate() {
         _tran.position = Vector3.SmoothDamp(_tran.position, targetToLook.position + offset, ref velo, smoothTime);
     }*/

}
