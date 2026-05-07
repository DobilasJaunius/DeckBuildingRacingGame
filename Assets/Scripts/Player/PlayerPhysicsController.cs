using UnityEngine;
using UnityEngine.ProBuilder;
using SF = UnityEngine.SerializeField;

public class PlayerPhysicsController : MonoBehaviour
{
    [SF] private Rigidbody rb;

    [SF] private float forceMulti = 30;
    [SF] private float dampStrength = 4f;
    [SF] private float moveSpeed = 18f;
    [SF] private float maxMoveSpeed = 20f;

    private float tripTime = 0f;
    private bool tripped = false;

    private bool canMove = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tripTime = 2f;
        }

        if(tripTime > 0)
        {
            tripTime -= Time.deltaTime;
            tripped = true;
            canMove = false;
        }
        else
        {
            tripped = false;
            canMove = true;
        }
    }

    void FixedUpdate()
    {
        if (!tripped)
        {
            TryStayUpwards();
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(input.magnitude != 0 && canMove)
        {
            PhysMove(input);
        }
    }

    private void TryStayUpwards()
    {
        Vector3 worldUp = Vector3.up;
        Vector3 localUp = transform.up;

        Vector3 pushVector = Vector3.Cross(localUp, worldUp);

        rb.AddTorque(pushVector * forceMulti, ForceMode.Force);
        rb.AddTorque(-rb.angularVelocity * dampStrength, ForceMode.Force);
    }

    private void PhysMove(Vector2 input)
    {
        Vector2 nInput = input.normalized;
        Debug.Log(input);

        Vector3 moveDir = Vector3.forward * nInput.y + Vector3.right * nInput.x;
        moveDir.y = 0;
        moveDir.Normalize();

        rb.AddForce(moveDir * moveSpeed, ForceMode.Force);

        Vector3 flat = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if(flat.magnitude > maxMoveSpeed)
        {
            flat = Vector3.ClampMagnitude(flat, maxMoveSpeed);
            rb.linearVelocity = new Vector3(flat.x, rb.linearVelocity.y, flat.z);
        }
    }
}
