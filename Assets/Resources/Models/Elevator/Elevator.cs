using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    Animator anim;
    public AudioClip elevator;
    AudioSource aSource;
    public bool activateOnStand = false;
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
        if (activateOnStand && !anim.GetBool("extended") && other.gameObject.name=="Player")
        {
            Extend();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (activateOnStand && anim.GetBool("extended") && other.gameObject.name == "Player")
        {
            Retract();
        }

    }
    public void Extend()
    {
        anim.SetBool("extended", true);
    }
    public void Retract()
    {
        anim.SetBool("extended", false);
    }
    public void PlaySound()
    {
        aSource.clip = elevator;
        aSource.Play();
    }
}
