using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

public class CarController : MonoBehaviour
{
    [SF] Rigidbody carRb;
    [SF] List<Transform> wheels;
    [SF] LayerMask drivable;

    [Header("Suspention")]
    [SF] float suspentionRestDist;
    [SF] float wheelRadius;
    [SF] float springStrength;
    [SF] float springDamper;

    [Header("Rotation")]
    [SF] float tireGripFactor;
    [SF] float tireMass;

    [Header("Controlls")]
    [SF] InputAction accelerate;
    [SF] AnimationCurve powerCurve;
    [SF] float carTopSpeed;
    [SF] float powerCurveMagnitude;
    //[SF] float max


    void Awake()
    {
        accelerate.Enable();
    }
    private void FixedUpdate()
    {
        Suspention();
        Steer();
        Drive();
    }

    private void Suspention()
    {
        foreach (var tireTransform in wheels)
        {
            RaycastHit tireRayHit;
            if(Physics.Raycast(tireTransform.position, -tireTransform.up, out tireRayHit, suspentionRestDist+wheelRadius, drivable))
            {
                Vector3 springDir = tireTransform.up;  
                Vector3 tireWorldVelocity = carRb.GetPointVelocity(tireTransform.position);

                float offset = (suspentionRestDist+wheelRadius) - tireRayHit.distance;

                float vel = Vector3.Dot(springDir, tireWorldVelocity);
                float force = (offset * springStrength) - (vel * springDamper);

                carRb.AddForceAtPosition(springDir * force, tireTransform.position);
            }

            Debug.DrawLine(tireTransform.position, tireTransform.position-tireTransform.up*(suspentionRestDist+wheelRadius), Color.green);
        }
    }

    private void Steer()
    {
        foreach (var tireTransform in wheels)
        {
            //If tire on the ground
            RaycastHit tireRayHit;
            if(Physics.Raycast(tireTransform.position, -tireTransform.up, out tireRayHit, suspentionRestDist+wheelRadius, drivable))
            {
                Vector3 steeringDir = tireTransform.right;
                Vector3 tireWorldVelocity = carRb.GetPointVelocity(tireTransform.position);

                float steeringVel = Vector3.Dot(steeringDir, tireWorldVelocity);

                float desiredVelChange = -steeringVel * tireGripFactor;
                float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

                carRb.AddForceAtPosition(steeringDir * tireMass * desiredAccel, tireTransform.position);
            }
        }
    }

    private void Drive()
    {
        foreach (var tireTransform in wheels)
        {
            //If tire on the ground
            RaycastHit tireRayHit;
            if(Physics.Raycast(tireTransform.position, -tireTransform.up, out tireRayHit, suspentionRestDist+wheelRadius, drivable))
            {
                Vector3 accelDir = tireTransform.forward;

                if(Mathf.Abs(accelerate.ReadValue<float>()) > 0.0f)
                {
                    float carSpeed = Vector3.Dot(carRb.transform.forward, carRb.linearVelocity);
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

                    float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelerate.ReadValue<float>() * powerCurveMagnitude;

                    carRb.AddForceAtPosition(accelDir * availableTorque, tireTransform.position);
                }
            }
        }
    }
}
