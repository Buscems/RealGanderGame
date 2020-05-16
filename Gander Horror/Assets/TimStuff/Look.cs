using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;


public class Look : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public float sensitivity;

    public Transform body;

    float Xrotation = 0f;

    bool usingController;

    Vector2 lookDir;

    private void Awake()
    {
        //Rewired Code
        myPlayer = ReInput.players.GetPlayer(playerNum - 1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(myPlayer.controllers.joystickCount > 0)
        {
            usingController = true;
        }
        else
        {
            usingController = false;
        }
    }

    
    void Update()
    {
        if (usingController)
        {
            lookDir.x = myPlayer.GetAxis("LookHorizontal") * sensitivity * Time.deltaTime;
            lookDir.y = myPlayer.GetAxis("LookVertical") * sensitivity * Time.deltaTime;
        }
        else
        {
            lookDir.x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            lookDir.y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        }
        Xrotation -= lookDir.y;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);

        body.Rotate(Vector3.up * lookDir.x);
    }
    //[REWIRED METHODS]
    //these two methods are for ReWired, if any of you guys have any questions about it I can answer them, but you don't need to worry about this for working on the game - Buscemi
    void OnControllerConnected(ControllerStatusChangedEventArgs arg)
    {
        usingController = true;
    }
    void OnControllerDisconnected(ControllerStatusChangedEventArgs arg)
    {
        usingController = false;
    }
}
