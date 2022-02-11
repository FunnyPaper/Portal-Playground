using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalUtility.Logic;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(Animator))]
public class DoorMechanism : Mechanism
{
    Animator anim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        _audioSource.clip = Resources.Load<AudioClip>(@"Sounds/door_open_clip");
        Check();
    }
    
    public void Open()
    {
        anim.SetBool("isOpen", true);
        _audioSource.Play();
    }
    public void Close()
    {
        anim.SetBool("isOpen", false);
        _audioSource.Play();
    }

    public override void Check()
    {
        if (InputSignals.Count < 1)
            return;

        bool currentCheck = true;

        currentCheck = LogicFunctions.EvaluateFunction(
            InputSignals.Select(signal => signal.IsActive),
            LogicType);

        //Jeśli jednak przejdzie test to jednorazowo zmień stan
        if (currentCheck != _check)
        {
            if (currentCheck)
            {
                Debug.Log("TRUE!");
                Open();
            }
            else
            {
                Debug.Log("FALSE!");
                Close();
            }
            _check = currentCheck;
        }
    }
}
