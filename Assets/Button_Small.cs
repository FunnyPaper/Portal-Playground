using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Small : MonoBehaviour
{
    Animator anim;
    AudioSource aSource;
    public AudioClip button_press;

    public List<GameObject> OnPress = new List<GameObject>();
    public List<StandardActions> OnPressFunctions = new List<StandardActions>();

    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    public void Use()
    {
            for (int i = 0; i < OnPress.Count; i++)
            {
                OnPress[i].BroadcastMessage(OnPressFunctions[i].ToString());
            }
        anim.Play("button_small_press");
        aSource.clip = button_press;
        aSource.Play();
    }
}