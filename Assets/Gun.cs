using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility.InheritBehaviour;

public class Gun : MonoBehaviour
{
    public float gunDistance = 23.0f;
    public float gunForce = 20.0f;
    public float gunDamage = 100.0f;
    Camera PlayerCamera;
    public GameObject Bullet;

    public AudioClip laser_shot;
    AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = laser_shot;
        PlayerCamera = transform.parent.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            GetComponent<Animator>().Play("gun_shot");
        }
    }

    void Shot()
    {
        AudioSource.Play();
        GameObject bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(PlayerCamera.transform.forward * 100, ForceMode.Impulse);

        RaycastHit hit;
        Debug.DrawRay(PlayerCamera.transform.position, PlayerCamera.transform.TransformDirection(Vector3.forward) * gunDistance, Color.red);
        if(Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, gunDistance))
        {
            AIEntity entity;
            if(hit.transform.TryGetComponent(out entity))
            {
                entity.TakeDamage(gunDamage);
            }
            //hit.transform.gameObject.BroadcastMessage("TakeDamage", gunDamage, SendMessageOptions.DontRequireReceiver);
            
            Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
            if (rb && !hit.transform.CompareTag("Redirection_Cube"))
            {
                //rb.AddForce(PlayerCamera.transform.forward * gunForce, ForceMode.Impulse);
            }
        }
    }
}
