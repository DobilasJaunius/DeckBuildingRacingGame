using UnityEngine;

public class IgnoreSelfColliders : MonoBehaviour
{
    void OnEnable()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        for(int i = 0; i < colliders.Length; i++)
        {
            for(int j = i + 1; j < colliders.Length; j++)
            {
                Physics.IgnoreCollision(colliders[i], colliders[j], true);
            }
        }
    }
}
