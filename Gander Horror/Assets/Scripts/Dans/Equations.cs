using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Equations
{
    /// <summary>
    /// Rotate towards point
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="objectToRotate"></param>
    /// <param name="turnSpeed"></param>
    /// <returns></returns>
    public static Quaternion RotateTowardsObj(Vector3 destination, Transform objectToRotate, float turnSpeed)
    {
        Quaternion newAngle = new Quaternion();

        Vector3 targetDirection = (destination - objectToRotate.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        newAngle = Quaternion.RotateTowards(objectToRotate.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        newAngle = Quaternion.Euler(new Vector3(0, newAngle.eulerAngles.y, 0));

        return newAngle;
    }

    /// <summary>
    /// Get angle between where "viewpointActor" is looking, and a second point
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="viewpointActor"></param>
    /// <returns></returns>
    public static float GetAngleBetweenTwoPoints(Vector3 destination, Transform viewpointActor)
    {
        Vector3 targetDir = destination - viewpointActor.transform.position;
        float angle = Vector3.Angle(targetDir, viewpointActor.transform.forward);

        return angle;
    }

    /// <summary>
    /// Find quickest path to a location
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="startingNode"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public static Node[] GetQuickestPathToLocation(Transform actor, Node[] startingNodes, Node.Locations location)
    {
        Node[] path = null;
        Stack<Node> order = new Stack<Node>();
        float shortestPath = -1;

        bool foundLocation = false;

        for (int k = 0; k < startingNodes.Length; k++)
        {
            startingNodes[k].SetVisited(true);
            order.Push(startingNodes[k]);

            while (foundLocation == false && order.Count != 0)
            {
                Node newNode = null;
                Node[] newNeighbors = order.Peek().neighbors;

                for (int i = 0; i < newNeighbors.Length; i++)
                {
                    if (newNeighbors[i].CheckIfVisited() == false)
                    {
                        newNode = newNeighbors[i];
                        newNeighbors[i].SetVisited(true);
                        break;
                    }
                }

                //if location reached
                if (newNode != null)
                {
                    if (newNode.CheckForLocation(location) == true)
                    {
                        order.Push(newNode);
                        float currentPath = -1;
                        Node lastNode = null;

                        //get path distance
                        foreach (Node n in order)
                        {
                            if (lastNode != null)
                            {
                                if (currentPath == -1)
                                {
                                    currentPath = Vector3.Distance(n.gameObject.transform.position, lastNode.gameObject.transform.position);
                                }
                                else
                                {
                                    currentPath += Vector3.Distance(n.gameObject.transform.position, lastNode.gameObject.transform.position);
                                }
                            }
                            lastNode = n;
                        }

                        if (shortestPath == -1 || currentPath < shortestPath)
                        {
                            shortestPath = currentPath;

                            Stack<Node> reversedPath = new Stack<Node>();
                            while (order.Count != 0)
                            {
                                reversedPath.Push(order.Pop());
                            }

                            path = reversedPath.ToArray();
                        }
                    }
                    //if location not reached
                    else
                    {
                        order.Push(newNode);
                    }
                }
                else
                {
                    order.Pop();
                }

                if(order.Count > 0)
                {
                    foundLocation = order.Peek().CheckForLocation(location);
                }                
            }
        }

        return path;
    }
}
