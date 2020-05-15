using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired;

public class PlayerMovement : MonoBehaviour
{
    //Player player;
    public GameObject cameraObject;
    [SerializeField] int playerid;

    public CharacterController controller;
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

    void Start()
    {
        //player = ReInput.players.GetPlayer(playerid);
        useController = false;
        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, 2.95f, cameraObject.transform.position.z);

        targetY = transform.position.y - 0.7f;
        mainY = transform.position.y + 0.3f;
    }

    
    void Update()
    {
        if (useController == false)
        {

            UpPos = new Vector3(cameraObject.transform.position.x, mainY, cameraObject.transform.position.z);
            DownPos = new Vector3(cameraObject.transform.position.x, targetY, cameraObject.transform.position.z);

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

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
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isCrouch = false;
            }
        }

        if (useController)
        {
            
        }
    }
}
