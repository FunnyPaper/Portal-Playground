using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Big : MonoBehaviour
{
    Animator anim;
    AudioSource aSource;
    public AudioClip press_audio_clip;

    public List<GameObject> OnPressObjects = new List<GameObject>();
    public List<StandardActions> OnPressFunctions = new List<StandardActions>();
    public List<GameObject> OnReleaseObjects = new List<GameObject>();
    public List<StandardActions> OnReleaseFunctions = new List<StandardActions>();

    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Use();
    }

    private void OnTriggerExit(Collider other)
    {
        Use();
    }

    public void Use()
    {
        //AKCJA NA WCIŚNIĘCIU
        if (!anim.GetBool("isPressed"))
        {
            anim.SetBool("isPressed", true);

            for (int i = 0; i < OnPressObjects.Count; i++)
            {
                OnPressObjects[i].BroadcastMessage(OnPressFunctions[i].ToString());
            }
        }
        //AKCJA NA WYCIŚNIĘCIU
        else
        {
            anim.SetBool("isPressed", false);

            for (int i = 0; i < OnReleaseObjects.Count; i++)
            {
                OnReleaseObjects[i].BroadcastMessage(OnReleaseFunctions[i].ToString());
            }
        }
        aSource.clip = press_audio_clip;
        aSource.Play();
    }
}
