using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility;

public class ButtonSmall : ButtonSignal, Useable
{
    protected override void Start()
    {
        base.Start();
        _audioSource.clip = Resources.Load<AudioClip>(@"Sounds/button_press");
    }

    public void Use()
    {
        IsActive = !IsActive;
        _animator.Play("button_small_press");
        _audioSource.Play();
    }
}
