using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(AudioSource), typeof(BoxCollider))]
public class Acid : MonoBehaviour
{
    AudioSource aSource;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity;
        if(other.TryGetComponent<Entity>(out entity))
        {
            entity.Destroy();
            aSource.Play();
        }
        //other.BroadcastMessage("Destroy", null, SendMessageOptions.DontRequireReceiver);
    }

}
