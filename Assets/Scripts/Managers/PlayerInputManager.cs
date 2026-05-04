using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;
using HI = UnityEngine.HideInInspector;

public class PlayerInputManager : MonoBehaviour
{
    [SF] private InputActionReference steerInput;
    [SF] private InputActionReference accelInput;

    [HI] public float steerValue;
    [HI] public float accelValue;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        steerValue = steerInput.action.ReadValue<float>();
        accelValue = accelInput.action.ReadValue<float>();
    }
}
