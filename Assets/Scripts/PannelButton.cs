using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility;

public class PannelButton : ButtonSignal, Useable
{
    [SerializeField]
    char Symbol = '1';

    public bool settable = true;

    public delegate void CharEventHandler(PannelButton button, char symbol);
    public event CharEventHandler ButtonPressed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //_audioSource.clip = Resources.Load<AudioClip>(@"Sounds/button_press");
    }

    public void Use()
    {
        if(settable)
        {
            if(!IsActive)
            {
                if (!_animator.GetBool("isPressed"))
                {
                    _animator.SetBool("isPressed", true);
                }
                IsActive = true;
                ButtonPressed?.Invoke(this, Symbol);
            }
            //_audioSource.Play();
        }
    }
}
