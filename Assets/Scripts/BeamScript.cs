using UnityEngine;
using GlobalUtility.InheritBehaviour;

public class BeamScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        AIEntity entity;
        if(other.TryGetComponent(out entity))
        {
            entity.TakeDamage(5);
        }
    }
}
