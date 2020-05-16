using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class PlayerMovement : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public GameObject cameraObject;
    public CapsuleCollider colliderPlayer;

    public float speed;
    public float gravity;

    [SerializeField] [Range(0f, 4f)] float moveSpeed;

    public Transform groundCheck;
    public float groundDist;
    public LayerMask groundMask;

    bool isGrounded;

    public bool useController;
    bool isCrouch;

    Vector3 velocity;
    Vector3 UpPos;
    Vector3 DownPos;
    float targetY;
    float mainY;
    float velocityY = 0;
    float smoothTime = 0.1f;

    float setSpeed;

    Rigidbody rb;

    public bool toggleCrouch;

    private void Awake()
    {
        //Rewired Code
        myPlayer = ReInput.players.GetPlayer(playerNum - 1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        CheckController(myPlayer);
    }

    void Start()
    {
        useController = false;
        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, 2.95f, cameraObject.transform.position.z);

        targetY = transform.position.y - 0.7f;
        mainY = transform.position.y + 0.3f;
        setSpeed = speed;

        rb = GetComponent<Rigidbody>();

    }
    
    void Update()
    {
        /*
        if (useController == false)
        {
            
            UpPos = new Vector3(cameraObject.transform.position.x, mainY, cameraObject.transform.position.z);
            DownPos = new Vector3(cameraObject.transform.position.x, targetY, cameraObject.transform.position.z);

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;

            if (isCrouch)
            {
                colliderPlayer.height = 1.0f;

                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, DownPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
            }

            if (isCrouch == false)
            {
                colliderPlayer.height = 2.0f;

                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, UpPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouch = true;
                speed = speed - 4;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isCrouch = false;
                speed = setSpeed;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                speed = speed + 4;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                speed = setSpeed;
            }

        }

        if (useController)
        {
            
        }
        */

        Movement();

    }

    private void FixedUpdate()
    {
        FixedMovement();
    }

    void Movement()
    {
        velocity = new Vector3(myPlayer.GetAxis("MoveLeftRight"), velocityY, myPlayer.GetAxis("MoveForwardBack"));
        velocity = transform.worldToLocalMatrix.inverse * velocity;

        UpPos = new Vector3(cameraObject.transform.position.x, mainY, cameraObject.transform.position.z);
        DownPos = new Vector3(cameraObject.transform.position.x, targetY, cameraObject.transform.position.z);

        if (isCrouch)
        {
            colliderPlayer.height = 1.0f;

            cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, DownPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
        }
        else
        {
            colliderPlayer.height = 2.0f;

            cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, UpPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
        }

        if (!toggleCrouch)
        {
            if (myPlayer.GetButtonDown("Crouch"))
            {
                isCrouch = true;
                speed = speed - 4;
            }

            if (myPlayer.GetButtonUp("Crouch"))
            {
                isCrouch = false;
                speed = setSpeed;
            }
        }
        else
        {
            if (myPlayer.GetButtonDown("Crouch"))
            {
                isCrouch = !isCrouch;
                speed = speed == setSpeed ? speed = speed - 4 : speed = setSpeed;
            }
        }
    }

    void FixedMovement()
    {
        rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
    }

    void Crouch()
    {
        UpPos = new Vector3(cameraObject.transform.position.x, mainY, cameraObject.transform.position.z);
        DownPos = new Vector3(cameraObject.transform.position.x, targetY, cameraObject.transform.position.z);

        if (isCrouch)
        {
            colliderPlayer.height = 1.0f;

            cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, DownPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
        }
        else
        {
            colliderPlayer.height = 2.0f;

            //cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, Mathf.Lerp(cameraObject.transform.position.y, UpPos.y, moveSpeed * Time.deltaTime), cameraObject.transform.position.z);
        }

        if (!toggleCrouch)
        {
            if (myPlayer.GetButtonDown("Crouch"))
            {
                isCrouch = true;
                speed = speed - 4;
            }

            if (myPlayer.GetButtonUp("Crouch"))
            {
                isCrouch = false;
                speed = setSpeed;
            }
        }
        else
        {
            if (myPlayer.GetButtonDown("Crouch"))
            {
                isCrouch = !isCrouch;
                speed = speed == setSpeed ? speed = speed - 4 : speed = setSpeed;
            }
        }
    }

    //[REWIRED METHODS]
    //these two methods are for ReWired, if any of you guys have any questions about it I can answer them, but you don't need to worry about this for working on the game - Buscemi
    void OnControllerConnected(ControllerStatusChangedEventArgs arg)
    {
        CheckController(myPlayer);
    }

    void CheckController(Player player)
    {
        foreach (Joystick joyStick in player.controllers.Joysticks)
        {
            var ds4 = joyStick.GetExtension<DualShock4Extension>();
            if (ds4 == null) continue;//skip this if not DualShock4
            switch (playerNum)
            {
                case 4:
                    ds4.SetLightColor(Color.yellow);
                    break;
                case 3:
                    ds4.SetLightColor(Color.green);
                    break;
                case 2:
                    ds4.SetLightColor(Color.blue);
                    break;
                case 1:
                    ds4.SetLightColor(Color.red);
                    break;
                default:
                    ds4.SetLightColor(Color.white);
                    Debug.LogError("Player Num is 0, please change to a number > 0");
                    break;
            }
        }
    }

}
