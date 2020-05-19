using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigation : MonoBehaviour
{
    [SerializeField]
    private Node currentNode;
    private Vector3 currentDestination;

    private Rigidbody rb;

    [SerializeField]
    [Range(0, 10)]
    private float walkSpeed;
    [SerializeField]
    [Range(0, 10)]
    private float runSpeed;
    [SerializeField]
    [Range(0, 1000)]
    private float turnSpeed;
    [Header("")]
    [SerializeField]
    [Range(0, 180)]
    private float maxTurnRotation;
    [SerializeField]
    [Range(0, 1)]
    private float minDistanceToDestination;
    [SerializeField]
    [Range(0, 100)]
    [Tooltip("This is the percentage chance of the goose not choosing the node it just came from")]
    private float chanceForNewNode;
    public Node lastNode = null;

    [SerializeField]
    [Tooltip("The max amount of time the goose waits before moving to a new node")]
    private float maxPotentiaGooseWaitTime;

    private bool aggro = false;
    private bool movingTowardsDestination = true;

    public bool goToKitchen = false;
    public bool goToBathroom = false;
    public bool goToBedRoom = false;
    public bool goToLivingRoom = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDestination = currentNode.GetDestination();
    }

    void Update()
    {
        //Rotate towards destination
        if (movingTowardsDestination)
        {
            transform.rotation = Equations.RotateTowardsObj(currentDestination, this.transform, turnSpeed);
        }

        if(Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.H))
        {
            Node.Locations location = new Node.Locations();
            if (Input.GetKeyDown(KeyCode.K))
            {
                location = Node.Locations.Kitchen;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                location = Node.Locations.Living_Room;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                location = Node.Locations.Bathroom;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                location = Node.Locations.Bedroom;
            }

            Node[] pxxxath = null;
            //Node[] pxxxath2 = null;
            if (lastNode == null)
            {
                pxxxath = Equations.GetQuickestPathToLocation(this.transform, new Node[] { currentNode }, location);
            }
            else
            {
                pxxxath = Equations.GetQuickestPathToLocation(this.transform, new Node[] { currentNode, lastNode }, location);
            }

            if (pxxxath != null)
            {
                for (int i = 0; i < pxxxath.Length; i++)
                {
                    Debug.Log("THE PATH1: " + pxxxath[i]);
                }
            }
            else
            {
                Debug.Log("ERROR");
            }
        }

    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        //go to node
        if (movingTowardsDestination)
        {
            float movementSpeed = aggro ? runSpeed : walkSpeed;
            rb.MovePosition((transform.position + (transform.forward * movementSpeed * Time.deltaTime)));

            //if goose reaches its destination
            //if go to next destination if the current angle to the new destination exceeds var max turn rotation, the goose will turn before moving to next destination
            if (Vector3.Distance(currentDestination, transform.position) < minDistanceToDestination)
            {
                movingTowardsDestination = false;

                //Random: Go immediately to next destination or wait a bit
                int randNum = Random.Range(0, 2);
                switch (randNum)
                {
                    //go immediately
                    case 0:
                        FindNewNode();
                        break;

                    //wait a tad
                    case 1:
                        StartCoroutine(GooseWaitTime());
                        break;
                }
            }
        }

        //random chance for pause and look around, also possible redirection of node
    }

    private void FindNewNode()
    {
        Node tempNode = currentNode;
        currentNode = currentNode.GetNewNeighbor(lastNode, chanceForNewNode);
        lastNode = tempNode;
        currentDestination = currentNode.GetDestination();

        //turn goose before moving if necessary
        StartCoroutine(GooseTurnBeforeMoving());
    }

    private IEnumerator GooseTurnBeforeMoving()
    {
        float angle = Equations.GetAngleBetweenTwoPoints(currentDestination, this.transform);

        while (angle > maxTurnRotation)
        {
            transform.rotation = Equations.RotateTowardsObj(currentDestination, this.transform, turnSpeed);
            angle = Equations.GetAngleBetweenTwoPoints(currentDestination, this.transform);
            yield return null;
        }

        movingTowardsDestination = true;
    }

    private IEnumerator GooseWaitTime()
    {
        yield return new WaitForSeconds(Random.Range(1, maxPotentiaGooseWaitTime));
        FindNewNode();
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

