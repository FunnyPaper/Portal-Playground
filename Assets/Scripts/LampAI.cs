using UnityEngine;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(Rigidbody))]
public sealed class LampAI : Entity
{
    [SerializeField]
    AudioClip _breakSound;    
    
    [SerializeField]
    AudioClip _buzzSound;
    
    [SerializeField]
    float _breakForce = 5;

    [SerializeField]
    [Range(0, 1)]
    float _buzzSoundVolume = 0.1f;

    [SerializeField]
    [Range(0, 1)]
    float _breakSoundVolume = 0.1f;

    AudioSource _aSource;
    Rigidbody _rBody;

    // Start is called before the first frame update
    void Start()
    {
        _aSource = this.gameObject.AddComponent<AudioSource>();
        _aSource.clip = _buzzSound;
        _aSource.volume = _buzzSoundVolume;
        _aSource.spatialBlend = 1f;
        _aSource.loop = true;
        _aSource.Play();

        _rBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > _breakForce)
        {
            _aSource.clip = _breakSound;
            _aSource.volume = _breakSoundVolume;
            _aSource.loop = false;
            _aSource.Play();

            GetComponentInChildren<Light>().enabled = false;
            _rBody.useGravity = true;
            _rBody.isKinematic = false;
            this.enabled = false;
        }
    }
}
