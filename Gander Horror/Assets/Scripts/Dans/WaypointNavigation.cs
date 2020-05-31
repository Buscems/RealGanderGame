using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointNavigation : MonoBehaviour
{
    [Header("Nav Mesh")]
    [SerializeField] private Transform player;
    private NavMeshAgent navMeshAgent;

    [Header("Node Logic")]
    [SerializeField]
    private Node currentNode;
    private Vector3 currentDestination;
    private Stack<Node> aggroPath = new Stack<Node>();

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
    [SerializeField]
    [Range(0, 1000)]
    private float aggroTurnSpeed;
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
        navMeshAgent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();
        currentDestination = currentNode.GetDestination();
    }

    void Update()
    {
        //Rotate towards destination
        if (movingTowardsDestination)
        {
            transform.rotation = Equations.RotateTowardsObj(currentDestination, this.transform, ((aggro == true ? aggroTurnSpeed : turnSpeed)));
        }

        #region get path to location debug code
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetLocationOverride(Node.Locations.Kitchen);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetLocationOverride(Node.Locations.Living_Room);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            GetLocationOverride(Node.Locations.Bathroom);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            GetLocationOverride(Node.Locations.Bedroom);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetLocationOverride(Node.Locations.Main_Entrance);
        }
        #endregion

    }

    private void GetLocationOverride(Node.Locations _location)
    {
        KeyValuePair<float, Node[]> path1;
        KeyValuePair<float, Node[]> path2;
        Node[] shortestPath;

        if (lastNode == null)
        {
            path1 = Equations.GetQuickestPathToLocation(this.transform, currentNode, _location);
            shortestPath = path1.Value;
        }
        else
        {
            path1 = Equations.GetQuickestPathToLocation(this.transform, currentNode, _location);
            path2 = Equations.GetQuickestPathToLocation(this.transform, lastNode, _location);

            //find which of the two paths are shorter when taking into account the gooses current position
            float path1Length = (path1.Key) + Mathf.Abs(Vector3.Distance(currentNode.transform.position, transform.position));
            float path2Length = (path2.Key) + Mathf.Abs(Vector3.Distance(lastNode.transform.position, transform.position));

            if (path1Length < path2Length)
            {
                shortestPath = path1.Value;
            }
            else
            {
                shortestPath = path2.Value;
            }
        }

        //print the path
        if (shortestPath != null)
        {
            for (int i = 0; i < shortestPath.Length; i++)
            {
                Debug.Log("THE PATH1: " + shortestPath[i]);
            }
        }
        else
        {
            Debug.Log("ERROR");
        }

        HurryToPlayer(shortestPath);
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

                if (!aggro) //if not aggro
                {
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
                else //if aggro
                {
                    lastNode = aggroPath.Peek();
                    aggroPath.Pop();
                    if (aggroPath.Count > 0)
                    {
                        currentDestination = aggroPath.Peek().GetDestination();
                        currentNode = aggroPath.Peek();
                        StartCoroutine(GooseTurnBeforeMoving()); 
                    }
                    else
                    {
                        aggro = false;
                        //investigate room behavior?

                        //temp
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

        while (angle > (aggro ? maxTurnRotation/2 : maxTurnRotation))
        {
            transform.rotation = Equations.RotateTowardsObj(currentDestination, this.transform, ((aggro == true ? aggroTurnSpeed : turnSpeed)));
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
    public void HurryToPlayer(Node[] path)
    {
        //play HONK! (very loud)
        aggro = true;
        movingTowardsDestination = false;
        lastNode = currentNode;
        aggroPath = new Stack<Node>(path);
        currentDestination = aggroPath.Peek().GetDestination();
        currentNode = aggroPath.Peek();
        StartCoroutine(GooseTurnBeforeMoving());
    }

    private void UpdateNavMesh()
    {
        navMeshAgent.SetDestination(player.position);
    }
}

