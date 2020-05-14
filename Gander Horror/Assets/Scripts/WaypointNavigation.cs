using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigation : MonoBehaviour
{
    public Vector3 currentDestination;

    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {

    }

    //stop and reconsider movement ? wait and go back to path : WALK to a node nearby the noise heard
    //For when player makes minor noise
    public void ReconsiderMovement(Node destination)
    {

    }

    //When player makes a loud noise forget current destination and RUN to the node nearest the noise made, starts walking when he reaches point
    //-very brief pause tho
    public void HurryToPlayer(Node Destination)
    {

    }
}

