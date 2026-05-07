using UnityEngine;
using SF = UnityEngine.SerializeField;

[RequireComponent(typeof(Rigidbody))]
public class TrackPosition : MonoBehaviour
{
    [SF] private Transform trackingPoint;
    [SF] private Rigidbody rb;
    [SF] private Collider col;

    [SF] private float pushForce = 400f;
    [SF] private float dampForce = 40f;

    [SF] private float rotationSpeed = 40;

    private float distanceToPoint = 0f;
    [SF] private float maxDistance = 3f;
    [SF] private float minDistance = 1f;

    public bool Track {get; set;}

    void OnEnable()
    {
        Track = true;
    }

    void OnValidate()
    {
        rb ??= GetComponent<Rigidbody>();
        col ??= GetComponent<Collider>();
    }

    public Vector3 GetTrackingPos()
    {
        return trackingPoint.position;
    }

    void Update()
    {
        if(distanceToPoint > maxDistance)
        {
            col.enabled = false;
        }else if(distanceToPoint < minDistance)
        {
            col.enabled = true;
        }

    }

    void FixedUpdate()
    {
        if(Track)
            StayOnTarget();
            MatchRotation();
    }

    private void StayOnTarget()
    {
        Vector3 currPos = rb.position;
        Vector3 targetPos = trackingPoint.position;

        Vector3 pushVector = targetPos - currPos;
        distanceToPoint = pushVector.magnitude;

        rb.AddForce(pushVector * pushForce, ForceMode.Force);
        rb.AddForce(-rb.linearVelocity * dampForce, ForceMode.Force);
    }

    private void MatchRotation()
    {
        Quaternion targetRot = Quaternion.Slerp(rb.rotation, trackingPoint.rotation, Time.fixedDeltaTime * rotationSpeed);
        rb.MoveRotation(targetRot);
    }
}
