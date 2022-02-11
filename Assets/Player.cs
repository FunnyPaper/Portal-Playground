using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource), typeof(CharacterController), typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    float rotX;
    float rotY;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    public int jumpFactor = 4;
    public float speed = 3.2f;
    [HideInInspector] public Vector3 movement;
    public CharacterController ctrl;
    
    [HideInInspector] public bool Flying = false;

    float ActionDistance = 3.0f;
    public GameObject hand;
    public GameObject grabbedObject;

    public Image Fader;
    
    [SerializeField]
    float _health = 200.0f;

    [SerializeField]
    float _maxHealth = 200.0f;

    public bool isAlive = true;
    private bool isAttacked = false;
    private bool isRegenerating = false;

    [SerializeField]
    float _regenAmountPerSec = 5;

    [SerializeField]
    Camera PlayerCamera;
    Rigidbody body;

    AudioSource aSource;
    Vector3 old_osition;
    public float step_size = 1.2f;
    public List<AudioClip> stepAudioClipList = new List<AudioClip>();
    public List<AudioClip> jumpAudioClipList = new List<AudioClip>();

    public AudioSource jump_aSource;
    float oldHeight = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        ctrl = GetComponent<CharacterController>();
        body = GetComponent<Rigidbody>();
        aSource = GetComponent<AudioSource>();
        StartCoroutine(ScheduleRegeneration());
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (ctrl.isGrounded)
                Flying = false;

            KeyboardMovement();
            MouseMovement();
            Use();
            PlayJumpMusic();

            if (transform.position.y < -10)
                Kill();
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
        if(!ctrl.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.Space) && ctrl.isGrounded)
        {
            Jump(new Vector3(movement.x, jumpFactor, movement.z));
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 8.0f;
            step_size = 2;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 3.2f;
            step_size = 1.2f;
        }
        if(!Flying)
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
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * ActionDistance, Color.red);
            Debug.DrawRay(transform.position, transform.forward * ActionDistance, Color.red);
            if (!grabbedObject)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, ActionDistance))
                {
                    if (hit.transform.gameObject.GetComponent<Grippable>())
                    {
                        grabbedObject = hit.transform.gameObject;
                        grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                        //grabbedObject.GetComponent<Grippable>().Use(hand);
                    }
                    else
                    {
                        if(!hit.transform.GetComponent<Button_Big>())
                        {
                            Debug.Log(hit.transform.name);
                            hit.transform.gameObject.BroadcastMessage("Use", null, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
            else
            {
                grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                //grabbedObject.transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0, transform.forward.z).normalized, Vector3.up);
                grabbedObject = null;
            }
        }
    }

    public void Kill()
    {
        isAlive = false;
        body.isKinematic = false;
        body.drag = 3;
        body.angularDrag = 3;
        body.useGravity = true;
        GetComponent<SphereCollider>().isTrigger = false;
        Debug.Log("Przegrales!");
        this.GetComponentInChildren<Gun>().enabled = false;
        Fader.GetComponent<Animator>().SetBool("IsAlive", false);
    }

    public void TakeDamage(float damage)
    {
        if (isAlive)
        {
            isAttacked = true;

            _health -= damage;
            //Debug.Log("Health: " + health);
            if (_health <= 0)
            {
                Kill();
            }
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
        if (Vector3.Distance(transform.position, old_osition) > step_size)
        {
            PlaySound(stepAudioClipList);
            old_osition = transform.position;
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

    public void Destroy()
    {
        Kill();
        //Destroy(this.gameObject);
    }
}
