using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtility.InheritBehaviour;

public class LaserSocket : Signal
{
    [SerializeField]
    float _bulletSignalTimeInSeconds = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<BeamScript>())
            return;

        if (!_animator.GetBool("isPressed"))
        {
            _animator.SetBool("isPressed", true);
            IsActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<BeamScript>())
            return;

        if (_animator.GetBool("isPressed"))
        {
            _animator.SetBool("isPressed", false);
            IsActive = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Bullet" && !_animator.GetBool("isPressed"))
        {
            StartCoroutine(ScheduleSignal(_bulletSignalTimeInSeconds));
        }
    }

    private IEnumerator ScheduleSignal(float seconds)
    {
        IsActive = true;
        _animator.SetBool("isPressed", true);

        yield return new WaitForSeconds(seconds);

        IsActive = false;
        _animator.SetBool("isPressed", false);
    }
}
