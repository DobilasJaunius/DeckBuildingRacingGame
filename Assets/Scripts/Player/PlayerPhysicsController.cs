using UnityEngine;
using SF = UnityEngine.SerializeField;

public class PlayerPhysicsController : MonoBehaviour
{
    [SF] private Rigidbody rb;
    [SF] Transform movePos;

    [SF] private float forceMulti = 30;
    [SF] private float dampStrength = 4f;
    [SF] private float moveSpeed = 4f;

    private float tripTime = 0f;
    private bool tripped = false;

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
        }
        else
        {
            tripped = false;
        }
    }

    void FixedUpdate()
    {
        if (!tripped)
        {
            TryStayUpwards();
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(input.magnitude != 0)
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

        Vector3 moveDir = movePos.forward * nInput.y + movePos.right * nInput.x;

        rb.AddForceAtPosition(moveDir * moveSpeed, movePos.position, ForceMode.Force);
    }
}
