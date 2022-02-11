using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    Animator anim;
    public GameObject jumpDirection;
    public float jumpForce = 3.0f;
    public AudioClip JumpAudioClip;
    AudioSource aSource;
    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {     
        aSource.clip = JumpAudioClip;
        aSource.Play();
        PlayerAI player;
        if (other.TryGetComponent(out player))
        {
            player.ctrl.Move(new Vector3(0, 0, 0));
            player.Jump((jumpDirection.transform.position - transform.position) * jumpForce);
            anim.Play("jump");
            player.Flying = true;
        }
        else if (!other.GetComponent<AutoDestruction>())
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce((jumpDirection.transform.position - transform.position) * jumpForce, ForceMode.Impulse);
            anim.Play("jump");
        }
    }
}
