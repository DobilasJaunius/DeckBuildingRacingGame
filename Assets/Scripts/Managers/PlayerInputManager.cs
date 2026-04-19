using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;
using HI = UnityEngine.HideInInspector;

public class PlayerInputManager : MonoBehaviour
{
    [SF] private InputActionReference steerInput;

    [HI] public float steerValue;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        steerValue = steerInput.action.ReadValue<float>();
    }
}
