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
        List<Stack<Node>> goodPaths = new List<Stack<Node>>();
        //List<Stack<Node>> allPaths = new List<Stack<Node>>();
        List<string> allPaths = new List<string>();
        Stack<Node> order = new Stack<Node>();
        float shortestPath = -1;
        int whileCount = 0;

        for (int k = 0; k < startingNodes.Length; k++)
        {
            startingNodes[k].SetVisited(true);
            order.Push(startingNodes[k]);

            //check if currently in room
            if (startingNodes[k].CheckForLocation(location) == true)
            {
                return new Node[] { startingNodes[k] };
            }                      

            while (order.Count != 0 && whileCount < 100)
            {
                Node newNode = null;
                Node[] newNeighbors = order.Peek().neighbors;

                for (int i = 0; i < newNeighbors.Length; i++)
                {
                    Stack<Node> tempOrder = new Stack<Node>(order);
                    tempOrder.Push(newNeighbors[i]);
                    string stringTempOrder = "";
                    foreach(Node n in tempOrder)
                    {
                        stringTempOrder += n.gameObject.name;
                    }

                    if (/*!allPaths.Contains(tempOrder)*/!allPaths.Contains(stringTempOrder) && newNeighbors[i].CheckIfVisited() == false)
                    {                       
                        allPaths.Add(stringTempOrder);
                        newNode = newNeighbors[i];
                        break;
                    }
                }

                //if location reached
                if (newNode != null)
                {
                    if (newNode.CheckForLocation(location) == true)
                    {
                        order.Push(newNode);
                        order.Peek().SetVisited(false);
                        goodPaths.Add(order);
                        order.Pop();

                        Debug.Log("POP1");
                    }
                    //if location not reached
                    else
                    {
                        Debug.Log("POP2");
                        order.Push(newNode);
                        order.Peek().SetVisited(true);
                    }
                }
                else
                {
                    Debug.Log("POP3");
                    Debug.Log("Before: " + order.Count);
                    string stringTempOrder = "";
                    foreach (Node n in order)
                    {
                        stringTempOrder += n.gameObject.name;
                    }
                    allPaths.Add(stringTempOrder);
                    order.Peek().SetVisited(false);
                    order.Pop();
                    Debug.Log("After: " + order.Count);
                }

                whileCount++;
            }
        }

        //get shortest path
        //sift through all node arrays
        for(int i = 0; i < goodPaths.Count; i++)
        {
            float distance = -1;
            Node lastNode = null;

            List<Node> tempStack = new List<Node>(goodPaths[i]);
            //sift through each element of node array
            for (int k = 0; k < tempStack.Count; k++)
            {
                if (lastNode != null)
                {
                    if (k == 0)
                    {
                        distance = Vector3.Distance(tempStack[k].gameObject.transform.position, lastNode.gameObject.transform.position);
                    }
                    else
                    {
                        distance += Vector3.Distance(tempStack[k].gameObject.transform.position, lastNode.gameObject.transform.position);
                    }
                }
                lastNode = tempStack[k];
            }

            if (shortestPath == -1 || distance < shortestPath)
            {
                shortestPath = distance;

                Stack<Node> reversedPath = new Stack<Node>();
                while (tempStack.Count != 0)
                {
                    // tempStack.Peek().SetVisited(false);
                    Stack<Node> finalStack = new Stack<Node>(tempStack);
                    reversedPath.Push(finalStack.Pop());
                }

                path = reversedPath.ToArray();
            }
        }

        return path;
    }
}
