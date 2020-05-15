using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigation : MonoBehaviour
{
    public Node currentNode;
    private Vector3 currentDestination;

    private Rigidbody rb;

    [Range(0,100)]
    public float walkSpeed;
    [Range(0, 100)]
    public float runSpeed;
    [Range(0, 1000)]
    public float turnSpeed;

    private bool aggro = false;
    private bool movingTowardsDestination = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDestination = currentNode.GetDestination();
    }

    void Update()
    {
        if (movingTowardsDestination)
        {
            Vector3 targetDirection = (currentDestination - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        //go to node
        float movementSpeed = aggro ? runSpeed : walkSpeed;
        rb.MovePosition((transform.position + (transform.forward * movementSpeed * Time.deltaTime)));

        //random chance for pause and look around, also possible redirection of node
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

