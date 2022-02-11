using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    public AudioClip door_open_clip;

    AudioSource aSource;
    public List<GameObject> ObjectToOvserveList = new List<GameObject>();
    public List<bool> ObjectStateCheckboxList = new List<bool>();
    bool spelnione = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Check();
    }
    public void Open()
    {
        anim.SetBool("isOpen", true);
        aSource.clip = door_open_clip;
        aSource.Play();
    }
    public void Close()
    {
        anim.SetBool("isOpen", false);
        aSource.clip = door_open_clip;
        aSource.Play();
    }
    public void Check()
    {
        bool currentSpelnione = true;
        if (ObjectToOvserveList.Count < 1 || ObjectStateCheckboxList.Count < 1)
        {
            return;
        }
        for (int i = 0; i < ObjectToOvserveList.Count; i++)
        {
            //Jeśli w którymś obiekcie obserwowanym jest ustawione isPressed nie tak jak trzeba to wyjdź z funkcji
            if (ObjectToOvserveList[i].GetComponent<Animator>().GetBool("isPressed") != ObjectStateCheckboxList[i])
            {
                currentSpelnione = false;
            }
        }

        //Jeśli jednak przejdzie test to jednorazowo zmień stan
        if (currentSpelnione != spelnione)
        {
            if (currentSpelnione)
            {
                Debug.Log("TRUE!");
                Open();
            }
            else
            {
                Debug.Log("FALSE!");
                Close();
            }
            spelnione = currentSpelnione;
        }
    }
}
