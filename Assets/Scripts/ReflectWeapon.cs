using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ReflectWeapon : MonoBehaviour
{
    [SerializeField]
    private Transform _reflectPoint;
    private Rigidbody _rb;

    public Transform ReflectPoint => _reflectPoint;

    [SerializeField]
    float _extendsBulletLifetime = 5;

    public int ReflectPrecision = 2;
    private float _multiplier = 10;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        for (int i = 0; i < ReflectPrecision; i++)
            _multiplier *= 10f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Bullet"))
            return;

        Rigidbody rb;
        if(collision.transform.TryGetComponent(out rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.transform.position = ReflectPoint.position;
            rb.transform.rotation = ReflectPoint.rotation;

            Vector3 forward = new Vector3(
                    Mathf.Round(ReflectPoint.forward.x * _multiplier) / _multiplier, 
                    Mathf.Round(ReflectPoint.forward.y * _multiplier) / _multiplier, 
                    Mathf.Round(ReflectPoint.forward.z * _multiplier) / _multiplier
                );

            rb.AddForce(forward * 100, ForceMode.Impulse);
        }

        collision.transform.GetComponent<AutoDestruction>().life_time = _extendsBulletLifetime;
    }
}
