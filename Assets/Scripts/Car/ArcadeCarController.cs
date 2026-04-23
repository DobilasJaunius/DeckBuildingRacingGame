using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ArcadeCarController : MonoBehaviour
{
    [SF] private Rigidbody rb;
    [SF] private List<Transform> wheels;
    [SF] private LayerMask ground;
    [SF] private PlayerInputManager inputManager;

    [Header("Suspension")]
    [SF] private float springStrength;
    [SF] private float springDamper;
    [SF] private float suspensionLength;

    [Header("Tyres")]
    [SF] private float gripValue;
    [SF] private float tyreMass;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        rb.AddForceAtPosition(_dir * tyreMass * acceleration, wheel.position);
    }

    void Steering()
    {
        foreach(Transform wheel in wheels)
        {
            if(wheel.tag == "frontWheel")
            {
                wheel.Rotate(Vector3.up, 45 * inputManager.steerValue);
                Mathf.Clamp(wheel.eulerAngles.y, -45, 45);
                Debug.Log(wheel.eulerAngles.y);
            }
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
