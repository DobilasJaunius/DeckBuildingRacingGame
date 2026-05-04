using UnityEngine;
using Mirror;
using SF = UnityEngine.SerializeField;
using Unity.Cinemachine;

public class PlayerController : NetworkBehaviour
{
    [SF] private CinemachineCamera _cam;
    [SF] private Transform _model;

    public override void OnStartLocalPlayer()
    {
        _cam.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        if(!isLocalPlayer) return;

        _model.localRotation = Quaternion.Euler(_model.eulerAngles.x, _cam.transform.eulerAngles.y, _model.eulerAngles.z);
    }
}
