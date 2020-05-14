using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired;

public class PlayerMovement : MonoBehaviour
{
    //Player player;
    [SerializeField] GameObject cameraObject;
    [SerializeField] int playerid;

    public CharacterController controller;
    public CapsuleCollider colliderPlayer;

    public float speed;
    public float gravity;

    public Transform groundCheck;
    public float groundDist;
    public LayerMask groundMask;

    bool isGrounded;

    public bool useController;

    Vector3 velocity;
    
    void Start()
    {
        //player = ReInput.players.GetPlayer(playerid);
        useController = false;
        //cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, colliderPlayer.height, cameraObject.transform.position.z);
    }

    
    void Update()
    {
        if (useController == false)
        {

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

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                colliderPlayer.height = 1.0f;
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, colliderPlayer.height, cameraObject.transform.position.z);
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                colliderPlayer.height = 2.0f;
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, colliderPlayer.height, cameraObject.transform.position.z);
            }
        }

        if (useController)
        {
            
        }
    }
}
