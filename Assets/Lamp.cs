using UnityEngine;

public class Lamp : MonoBehaviour
{
    AudioSource aSource;
    public float breakForce = 5;
    public AudioClip electric_break;

    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > breakForce)
        {
            aSource.clip = electric_break;
            aSource.loop = false;
            aSource.Play();

            GetComponentInChildren<Light>().enabled = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.enabled = false;
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
