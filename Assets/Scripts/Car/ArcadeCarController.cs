using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ArcadeCarController : MonoBehaviour
{
    [Header("Refferences")]
    [SF] private Rigidbody rb;
    [SF] private List<Transform> wheels;
    [SF] private LayerMask ground;
    [SF] private PlayerInputManager inputManager;

    private float wheelRotation;

    [Header("Suspension")]
    [SF] private float springStrength;
    [SF] private float springDamper;
    [SF] private float suspensionLength;

    [Header("Tyres")]
    [SF] private float gripValue;
    [SF] private float tyreMass;

    [Header("Steering values")]
    [SF] private float steerSmooth;
    [SF] private float maxSteerAngle;

    [Header("Acceleration values")]
    [SF] private float maxSpeed;
    [SF] private float maxReverseSpeed;
    [SF] private float acceleration;
    [SF] private float brakeTorque;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CarInput();
    }

    void FixedUpdate() {
        WheelLogic();
        Steering();
    }

    void WheelLogic()
    {
        RaycastHit _hit;
        foreach(Transform wheel in wheels)
        {
            if(Physics.Raycast(wheel.position, -wheel.up, out _hit, suspensionLength, ground))
            {
                CalculateSuspension(wheel, _hit);
                CalculateTyreGrip(wheel);
                Acceleration(wheel);
            }
        }
    }

    void CalculateSuspension(Transform wheel, RaycastHit _hit)
    {
        
        Vector3 _dir = wheel.up;
        
        Vector3 _tireVel = rb.GetPointVelocity(wheel.position);
                
        float offset = suspensionLength - _hit.distance;

        float _vel = Vector3.Dot(_dir, _tireVel);

        float force = (offset * springStrength) - (_vel * springDamper);

        rb.AddForceAtPosition(force * _dir * Time.fixedDeltaTime, wheel.position);
    }

    void CalculateTyreGrip(Transform wheel)
    {
        Vector3 _dir = wheel.right;

        Vector3 _tireVel = rb.GetPointVelocity(wheel.position);

        float steeringVel = Vector3.Dot(_dir, _tireVel);

        float velChange = -steeringVel * gripValue;

        float acceleration = velChange / Time.fixedDeltaTime;

        rb.AddForceAtPosition(_dir * tyreMass * acceleration * Time.fixedDeltaTime, wheel.position);
    }

    void Steering()
    {
        foreach(Transform wheel in wheels)
        {
            

            if(wheel.tag == "frontWheel")
            {
                wheel.transform.rotation = Quaternion.Euler(wheel.transform.rotation.x, wheelRotation, wheel.transform.rotation.z);
            }
        }
    }

    void Acceleration(Transform wheel)
    {
        if(wheel.tag == "frontWheel")
        {
            Vector3 _dir = wheel.forward;
            float _speed = Vector3.Dot(transform.forward, rb.linearVelocity);

            if(inputManager.accelValue > 0f)
            {


                //float normalizeSpeed = Mathf.Clamp01(Mathf.Abs(_speed) / maxSpeed);

                rb.AddForceAtPosition(inputManager.accelValue * acceleration * _dir * Time.fixedDeltaTime, wheel.position);
            }

            else if(inputManager.accelValue < 0f)
            {
                float velChange = -_speed * gripValue;

                float brakeForce = velChange / Time.fixedDeltaTime;

                rb.AddForceAtPosition(inputManager.accelValue * brakeForce * _dir * Time.fixedDeltaTime, wheel.position);
            }
        }
    }

    void CarInput()
    {
        if(inputManager.steerValue != 0f)
        {
            wheelRotation = Mathf.Lerp(wheelRotation, maxSteerAngle, steerSmooth * Time.deltaTime);
        }
        else
        {
            wheelRotation = Mathf.Lerp(wheelRotation, 0, steerSmooth * Time.deltaTime);    
        }
    }

    //For debuging
    void OnDrawGizmosSelected()
    {
        foreach(Transform wheel in wheels)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(wheel.position, -wheel.up * suspensionLength);
        }
    }
}
