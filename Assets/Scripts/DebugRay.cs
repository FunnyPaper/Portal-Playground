using UnityEngine;

public class DebugRay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.cyan, 20000);
    }
}
