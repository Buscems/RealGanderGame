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

    public float speed;
    public float gravity;

    [SerializeField] [Range(0f, 4f)] float moveSpeed;

    //public Transform groundCheck;
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

    //Assingables
    public Transform playerCam;
    public Transform head;
    public Transform orientation;
    public CapsuleCollider colliderPlayer;
    public Transform leanLeft;
    public Transform leanRight;
    public Transform noLean;

    //Rotation and look
    private float xRotation;
    public float sensitivity;
    private float sensMultiplier = 1f;
    float lookX, lookY;
    private float desiredX;

    bool isLeaning;
    [SerializeField]
    Transform currentLean;

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

        targetY = transform.position.y - 0.5f;
        mainY = transform.position.y + 0.3f;
        setSpeed = speed;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentLean = noLean;

    }
    
    void Update()
    {

        if (Cursor.visible)
        {
            Cursor.visible = false;
        }

        Movement();

        Look();

        if (myPlayer.GetButtonDown("LeanLeft"))
        {
            currentLean = leanLeft;
        }
        else if (myPlayer.GetButtonDown("LeanRight"))
        {
            currentLean = leanRight;
        }
        else if(myPlayer.GetButtonUp("LeanLeft") || myPlayer.GetButtonUp("LeanRight"))
        {
            currentLean = noLean;
        }

        LeanCamera(currentLean);

    }

    private void FixedUpdate()
    {
        FixedMovement();
    }

    void Movement()
    {
        velocity = new Vector3(myPlayer.GetAxis("MoveLeftRight"), velocityY, myPlayer.GetAxis("MoveForwardBack"));
        velocity = orientation.transform.worldToLocalMatrix.inverse * velocity;

        Crouch();
    }

    void FixedMovement()
    {
        rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
        if(velocity == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void Look()
    {
        if (useController)
        {
            lookX = myPlayer.GetAxis("LookX") * sensitivity * Time.deltaTime * sensMultiplier;
            lookY = myPlayer.GetAxis("LookY") * sensitivity * Time.deltaTime * sensMultiplier;
        }
        else
        {
            lookX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * sensMultiplier;
            lookY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * sensMultiplier;
        }
        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + lookX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void LeanCamera(Transform lean)
    {

        head.transform.position = new Vector3(Mathf.Lerp(head.transform.position.x, lean.position.x, moveSpeed * Time.deltaTime), head.position.y, Mathf.Lerp(head.transform.position.z, lean.position.z, moveSpeed * Time.deltaTime));

    }

    void Crouch()
    {
        UpPos = new Vector3(head.transform.position.x, mainY, head.transform.position.z);
        DownPos = new Vector3(head.transform.position.x, targetY, head.transform.position.z);

        if (isCrouch)
        {

            colliderPlayer.height = 1.0f;
            colliderPlayer.center = new Vector3(0, -.5f, 0);

            head.transform.position = new Vector3(head.transform.position.x, Mathf.Lerp(head.transform.position.y, DownPos.y, moveSpeed * Time.deltaTime), head.transform.position.z);
        }
        else
        {
            colliderPlayer.height = 2.0f;
            colliderPlayer.center = new Vector3(0, 0, 0);

            head.transform.position = new Vector3(head.transform.position.x, Mathf.Lerp(head.transform.position.y, UpPos.y, moveSpeed * Time.deltaTime), head.transform.position.z);
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
