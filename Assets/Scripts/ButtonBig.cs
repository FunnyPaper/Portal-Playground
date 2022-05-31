using UnityEngine;
using GlobalUtility.InheritBehaviour;

public class ButtonBig : ButtonSignal
{
    protected override void Start()
    {
        base.Start();
        _audioSource.clip = Resources.Load<AudioClip>(@"Sounds/button_press");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Zdefiniowac tag system ?
        if (!other.GetComponent<Entity>())
            return;

        if (!_animator.GetBool("isPressed"))
        {
            _animator.SetBool("isPressed", true);
            IsActive = true;
            _audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Zdefiniowac tag system ?
        if (!other.GetComponent<Entity>())
            return;

        if (_animator.GetBool("isPressed"))
        {
            _animator.SetBool("isPressed", false);
            IsActive = false;
            _audioSource.Play();
        }
    }
}
