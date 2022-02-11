using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grippable : MonoBehaviour
{
    public bool isGrabbed = false;
    public GameObject Hand;
    Rigidbody body;

    AudioSource _aSource;

    [SerializeField]
    AudioClip _groundHitSound;

    float initialMass;
    float initialDrag;
    float initialAngularDrag;

    public Vector3 HandOffset = new Vector3(0, -1f, 0);

    void Awake()
    {
        body = GetComponent<Rigidbody>();

        initialMass = body.mass;
        initialDrag = body.drag;
        initialAngularDrag = body.angularDrag;

        _aSource = this.gameObject.AddComponent<AudioSource>();

        if (_groundHitSound)
            _aSource.clip = _groundHitSound;
        else
            _aSource.clip = Resources.Load<AudioClip>(@"Sounds/metal_hit_1");

        _aSource.playOnAwake = false;
        _aSource.spatialBlend = 1;
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            body.AddForce((Hand.transform.position - transform.position) * 100);
            transform.rotation = Quaternion.LookRotation(new Vector3(Hand.transform.forward.x - HandOffset.x, 0, Hand.transform.forward.z - HandOffset.z).normalized, Vector3.up);
        }
    }

    public void Use(GameObject _Hand = null)
    {
        Hand = _Hand;
        if (isGrabbed == false)
        {
            body.drag = 15.0f;
            body.angularDrag = 12.0f;
            body.mass = 0.1f;
            body.useGravity = false;
            body.isKinematic = false;
            transform.position = Hand.transform.position;
            isGrabbed = true;
        }
        else
        {
            body.useGravity = true;
            body.drag = initialDrag;
            body.angularDrag = initialAngularDrag;
            body.mass = initialMass;
            isGrabbed = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = collision.relativeVelocity.magnitude / 10;
        _aSource.volume = Mathf.Clamp(magnitude, 0, 1);
        _aSource.pitch = Random.Range(0.7f, 1.3f);
        _aSource.Play();
    }
}