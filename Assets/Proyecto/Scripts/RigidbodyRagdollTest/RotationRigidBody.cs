using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRigidBody : MonoBehaviour
{
    public Quaternion localDesiredRot;
    Quaternion newRot;
    Rigidbody rb = default;
    //Transform tr = default;
    [SerializeField] float fuerzadebalance = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //tr = transform;
    }

    private void Update()
    {
        //transform.rotation = Quaternion.Slerp(rb.rotation, localDesiredRot, fuerzadebalance * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, localDesiredRot, fuerzadebalance * Time.fixedDeltaTime));
    }

}
