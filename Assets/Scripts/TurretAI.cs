using System.Collections.Generic;
using UnityEngine;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(Animator))]
public class TurretAI : AIEntity
{
    public bool armed = false;
    public float turret_damage = 5;

    [Header("Game Objects References")]

    [SerializeField] GameObject Spine = null;
    [HideInInspector] public GameObject Player;
    [SerializeField] GameObject Laser = null;
    Animator anim;

    [Header("Sounds")]

    AudioSource speak_aSource;
    [SerializeField] List<AudioClip> turret_shoot_list = new List<AudioClip>();
    [SerializeField] AudioClip activated = null;
    [SerializeField] AudioClip deploying = null;
    [SerializeField] AudioClip there_you_are = null;
    [SerializeField] AudioClip i_see_you = null;
    [SerializeField] AudioClip anyone_there = null;
    [SerializeField] AudioClip searching = null;
    [SerializeField] AudioClip sentry_mode_activated = null;
    [SerializeField] AudioClip shutting_down = null;

    protected override void Start()
    {
        base.Start();
        Player = GameObject.Find("PlayerCamera");
        anim = GetComponent<Animator>();
        speak_aSource = this.gameObject.AddComponent<AudioSource>();
        speak_aSource.spatialBlend = 1;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (IsAlive)
        {
            SightCheck();

            if (armed)
            {
                Quaternion spineRotation = Quaternion.LookRotation(Player.transform.position - Spine.transform.position);
                Spine.transform.rotation = Quaternion.Slerp(Spine.transform.rotation, spineRotation, 5.0f * Time.deltaTime);
                Aim();

            }
            else
            {
                Spine.transform.rotation = Quaternion.Slerp(Spine.transform.rotation, this.transform.rotation, 1.0f * Time.deltaTime);
            }
        }
    }

    void SightCheck()
    {
        RaycastHit hit;
        Debug.DrawRay(Laser.transform.position, (Player.transform.position - Laser.transform.position), Color.green);
        if (Physics.Raycast(Laser.transform.position, (Player.transform.position - Laser.transform.position), out hit, Mathf.Infinity))
        {
            if (hit.transform.name == "Player")
            {
                if (!anim.GetBool("armed"))
                {

                    anim.SetBool("armed", true);
                    Speak(activated, there_you_are, i_see_you, deploying);

                }
            }
            else
            {
                if (anim.GetBool("armed"))
                {
                    anim.SetBool("armed", false);
                    anim.SetBool("shoot", false);
                    Speak(anyone_there, searching, sentry_mode_activated);

                }
            }
        }
    }

    public void Speak(params AudioClip[] audio_clip_list)
    {
        //wylosuj którąś z podanych próbek dźwiękowych
        int clipIndex = Random.Range(0, audio_clip_list.Length);
        AudioClip clip = audio_clip_list[clipIndex];
        speak_aSource.clip = clip;
        speak_aSource.Play();
    }

    public void SetArmedState(int state)
    {

        if (state == 1)
        {
            armed = true;
        }
        else
        {
            armed = false;
        }
    }

    void Aim()
    {
        RaycastHit hit;
        Debug.DrawRay(Laser.transform.position, Laser.transform.forward * 1000, Color.red);
        if (Physics.Raycast(Laser.transform.position, Laser.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.name == "Player")
            {
                anim.SetBool("shoot", true);
            }
            else
            {
                anim.SetBool("shoot", false);
            }
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        Debug.DrawRay(Laser.transform.position, Laser.transform.forward * 1000, Color.yellow);
        if (Physics.Raycast(Laser.transform.position, Laser.transform.forward, out hit, Mathf.Infinity))
        {
            PlayerAI player;
            if (hit.transform.TryGetComponent(out player))
            {
                player.TakeDamage(turret_damage);
                AudioSource shoot_aSource = this.gameObject.AddComponent<AudioSource>();
                AudioClip current_clip = turret_shoot_list[Random.Range(0, turret_shoot_list.Count)];
                shoot_aSource.clip = current_clip;
                shoot_aSource.spatialBlend = 1;
                shoot_aSource.Play();
                Destroy(shoot_aSource, current_clip.length);
            }
        }
    }

    protected override void Kill()
    {
        Laser.GetComponent<LineRenderer>().enabled = false;
        anim.Play("turret_disarmed");
        anim.SetBool("armed", false);
        anim.SetBool("shoot", false);
        armed = false;
        Speak(shutting_down);
        rBody.AddForce(Vector3.up * 250, ForceMode.Force);
        _health = 0;
        base.Kill();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 6 && IsAlive)
        {
            Kill();
        }
    }
}