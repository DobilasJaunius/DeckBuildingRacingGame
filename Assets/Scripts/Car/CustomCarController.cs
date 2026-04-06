using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

public class CustomCarController : MonoBehaviour
{
    //Short overall todo
    // 1. Traction force
    // 2. Drag F_drag = Cdrag * v*v
    // 3. Rolling resistance = F_rr = Crr * v
    // 4. Weight transfer mu * W

    [Header("References")]
    [SF] Rigidbody carRb;
    [SF] List<Transform> wheels;
    
    [Header("Constants")]
    [SF] float EngineForce;
    [SF] float Cdrag;
    [SF] float Crr;
    [SF] float Cbrake;
    [SF] float FrictionCoeff;

    [Header("Suspention")]
    [SF] float restLength;
    [SF] float springStiffness;
    [SF] float damperStrength;
    [SF] float wheelRadius;

    [Header("Weight transfer")]
    [SF] float wheelBase = 2f;    // distance front to rear axle
    [SF] float cgHeight = 0.5f;     // height of centre of mass
    [SF] float cgToRear = 0.75f;    // distance CG to rear axle (c)
    [SF] float cgToFront = 1.25f;   // distance CG to front axle (b)

    [Header("Input")]
    [SF] InputAction GasAction;

    Vector3 _Ftraction; //Traction force
    Vector3 _Fdrag; //Drag force
    Vector3 _Frr; //Rolling resistance
    Vector3 _Fbrake; //Braking force
    Vector3 _Fmax; //Friction limit for a wheel

    Vector3 _Flong;
    float _acc;
    float _W;
    float _Wf;
    float _Wr;
    float _maxTractionRear;
    float _maxTractionFront;
    Vector3 _lastVelocity;


    void Awake()
    {
        GasAction.Enable();
        _W = carRb.mass * 9.81f;
    }

    //Physics updates will be here
    void FixedUpdate()
    {
        Vector3 acceleration = (carRb.linearVelocity - _lastVelocity) / Time.fixedDeltaTime;
        _acc = Vector3.Dot(acceleration, transform.forward);

        CalculateWeightTransfer();
        CalculateForces();
        
        Debug.Log("acc: " + _acc);
        Debug.Log("Wr: " + _Wr);

        ApplyForces();

        _lastVelocity = carRb.linearVelocity;
    }

    private void ApplyForces()
    {
        if(GasAction.ReadValue<float>() > 0.0f)
        {
            Debug.Log(GasAction.ReadValue<float>());
            _Flong = _Ftraction + _Fdrag + _Frr;
        }
        else if(GasAction.ReadValue<float>() < 0.0f 
                && carRb.linearVelocity.magnitude > 0)
        {
            _Flong = _Fbrake + _Fdrag + _Frr;
        }
        else
        {
            _Flong = _Fdrag + _Frr;
        }

        carRb.AddForce(_Flong);
    }

    private void CalculateWeightTransfer()
    {
        _Wf = (cgToRear/wheelBase) * _W - (cgHeight/wheelBase) * carRb.mass * _acc;
        _Wr = (cgToFront/wheelBase) * _W + (cgHeight/wheelBase) * carRb.mass * _acc;

        _maxTractionFront = _Wf * FrictionCoeff;
        _maxTractionRear = _Wr * FrictionCoeff;
    }

    private void CalculateForces()
    {
        _Fdrag = -Cdrag * carRb.linearVelocity * carRb.linearVelocity.magnitude;
        _Frr = -Crr * carRb.linearVelocity;
        _Fbrake = -Cbrake * carRb.linearVelocity.normalized;
        _Ftraction = Mathf.Min(_maxTractionRear, EngineForce) * transform.forward;
    }
}
