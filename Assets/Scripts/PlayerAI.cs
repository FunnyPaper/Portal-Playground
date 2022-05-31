using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalUtility.InheritBehaviour;
using GlobalUtility;

[RequireComponent(typeof(CharacterController))]
public class PlayerAI : AIEntity
{
    float rotX;
    float rotY;

    [SerializeField] float mouseSensitivity = 100.0f;
    [SerializeField] float clampAngle = 80.0f;
    [SerializeField] int jumpFactor = 4;
    [SerializeField] float speed = 3.2f;
    public CharacterController ctrl;

    [HideInInspector] public Vector3 movement;
    [HideInInspector] public bool Flying = false;

    float ActionDistance = 3.0f;
    [SerializeField] GameObject hand = null;
    [SerializeField] GameObject grabbedObject = null;
    [SerializeField] Image Fader = null;
    [SerializeField] float _maxHealth = 200.0f;

    bool isAttacked = false;
    bool isRegenerating = false;

    [SerializeField] float _regenAmountPerSec = 5;
    [SerializeField] Camera PlayerCamera = null;

    AudioSource aSource;
    Vector3 old_position;
    [SerializeField] float step_size = 1.2f;
    [SerializeField] List<AudioClip> stepAudioClipList = new List<AudioClip>();
    [SerializeField] List<AudioClip> jumpAudioClipList = new List<AudioClip>();
    [SerializeField] AudioSource jump_aSource = null;
    float oldHeight = 0.0f;

    protected override void Start()
    {
        base.Start();
        ctrl = GetComponent<CharacterController>();
        aSource = GetComponent<AudioSource>();
        StartCoroutine(ScheduleRegeneration());
    }

    protected override void Update()
    {
        if (IsAlive)
        {
            if (ctrl.isGrounded)
                Flying = false;

            KeyboardMovement();
            MouseMovement();
            Use();
            PlayJumpMusic();

            base.Update();
        }
    }

    void MouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        rotY += mouseX * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }

    void KeyboardMovement()
    {
        if (!ctrl.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space) && ctrl.isGrounded)
        {
            Jump(new Vector3(movement.x, jumpFactor, movement.z));
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 8.0f;
            step_size = 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 3.2f;
            step_size = 1.2f;
        }
        if (!Flying)
        {
            movement.z = Input.GetAxis("Vertical") * speed * transform.forward.z - Input.GetAxis("Horizontal") * speed * transform.forward.x;
            movement.x = Input.GetAxis("Vertical") * speed * transform.forward.x + Input.GetAxis("Horizontal") * speed * transform.forward.z;
            Steps();
            oldHeight = transform.position.y;
        }
        ctrl.Move(movement * Time.deltaTime);
    }

    public void Jump(Vector3 jumpDirection)
    {
        movement = jumpDirection;
        PlaySound(jumpAudioClipList);
    }

    public void Use()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.DrawRay(transform.position, transform.forward * ActionDistance, Color.red);
            if (!grabbedObject)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, ActionDistance))
                {
                    Grippable grip;
                    Useable interaction;
                    if (hit.transform.gameObject.TryGetComponent(out grip))
                    {
                        grabbedObject = hit.transform.gameObject;
                        grip.Use(hand);
                        //grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                    }
                    else if(hit.transform.gameObject.TryGetComponent(out interaction))
                    {
                        interaction.Use();
                    }
                }
            }
            else
            {
                grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                grabbedObject = null;
            }
        }
    }

    protected override void Kill()
    {
        rBody.isKinematic = false;
        rBody.drag = 3;
        rBody.angularDrag = 3;
        rBody.useGravity = true;
        GetComponent<SphereCollider>().isTrigger = false;
        Debug.Log("Przegrales!");
        this.GetComponentInChildren<Gun>().enabled = false;
        Fader.GetComponent<Animator>().SetBool("IsAlive", false);
        base.Kill();
    }

    public override void TakeDamage(float damage)
    {
        if (IsAlive)
        {
            isAttacked = true;
            base.TakeDamage(damage);

            Fader.GetComponent<Animator>().Play("fader_red");
            PlayerCamera.GetComponent<Animator>().Play("camera_hit");

            StartCoroutine(ScheduleRegeneration());
        }
    }

    public void PlaySound(List<AudioClip> lista)
    {
        aSource.clip = lista[Random.Range(0, lista.Count)];
        aSource.Play();
    }

    public void Steps()
    {
        if (Vector3.Distance(transform.position, old_position) > step_size)
        {
            PlaySound(stepAudioClipList);
            old_position = transform.position;
            PlayerCamera.GetComponent<Animator>().Play("camera_walk", 0, 0);
        }
    }

    public void PlayJumpMusic()
    {
        float height = transform.position.y - oldHeight;
        jump_aSource.volume = height * height / 50.0f;
        if (ctrl.isGrounded)
        {
            oldHeight = transform.position.y;
        }
    }

    private IEnumerator ScheduleRegeneration()
    {
        yield return new WaitForSeconds(5);
        if (!isRegenerating)
        {
            isRegenerating = true;
            isAttacked = false;
            while (_health < _maxHealth && !isAttacked)
            {
                _health += _regenAmountPerSec;
                yield return new WaitForSeconds(1);
            }
            isRegenerating = false;
        }
    }

    public override void Destroy()
    {
        Kill();
    }
}