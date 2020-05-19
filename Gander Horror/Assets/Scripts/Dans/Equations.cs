using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

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

    public static KeyValuePair<float, List<Stack<Node>>> GetAllPaths(Stack<Node> _order, Node.Locations _location, float pathToShortestLocation, float _currentPathLength)
    {
        List<Stack<Node>> paths = new List<Stack<Node>>();
        Node[] neighbors = _order.Peek().neighbors;
        float shortestPath = pathToShortestLocation;
        float currentPathLength = _currentPathLength;
        
        #region Debug Display
        /*
        //print out the node path as it runs through
        Stack<Node> debugStack = new Stack<Node>(_order);
        while (debugStack.Count > 0)
        {
            Debug.Log(debugStack.Peek().gameObject.name);
            debugStack.Pop();
        }
        Debug.Log("=================================");
        */
        #endregion

        for (int i = 0; i < neighbors.Length; i++)
        {            
            #region method one SLOWER
            /*
            UnityEngine.Debug.Log("CheckPath");
            float newCurrentPathLength = currentPathLength;
            if (newCurrentPathLength != -1)
            {
                newCurrentPathLength += Mathf.Abs(Vector3.Distance(neighbors[i].gameObject.transform.position, _order.Peek().gameObject.transform.position));
            }
            else
            {
                newCurrentPathLength = Mathf.Abs(Vector3.Distance(neighbors[i].gameObject.transform.position, _order.Peek().gameObject.transform.position));
            }
            */
            #endregion           
            
            #region method two FASTER
            float newCurrentPathLength = -1;
            Node lastNode = null;

            //find path length
            List<Node> tempStack0 = new List<Node>(_order);
            //sift through each element of node array
            for (int k = 0; k < tempStack0.Count; k++)
            {
                if (lastNode != null)
                {
                    if (k == 0)
                    {
                        newCurrentPathLength = Mathf.Abs(Vector3.Distance(tempStack0[k].gameObject.transform.position, lastNode.gameObject.transform.position));
                    }
                    else
                    {
                        newCurrentPathLength += Mathf.Abs(Vector3.Distance(tempStack0[k].gameObject.transform.position, lastNode.gameObject.transform.position));
                    }
                }
                lastNode = tempStack0[k];
            }
            #endregion
            

            if (newCurrentPathLength < shortestPath || shortestPath == -1)
            {
                Stack<Node> tempStack = new Stack<Node>(_order);
                tempStack = new Stack<Node>(tempStack);

                //if the right location, add stack to list
                if (neighbors[i].CheckForLocation(_location) == true)
                {
                    tempStack.Push(neighbors[i]);
                    paths.Add(tempStack);
                    shortestPath = newCurrentPathLength;

                    #region Debug Display
                    //print out the node path as it runs through
                    Stack<Node> debugStack = new Stack<Node>(tempStack);
                    while (debugStack.Count > 0)
                    {
                        debugStack.Pop();
                    }
                    #endregion
                }
                else
                {
                    //if next neighbor hasnt been visited yet, check that path
                    if (neighbors[i].CheckIfVisited() == false)
                    {
                        neighbors[i].SetVisited(true);
                        tempStack.Push(neighbors[i]);

                        //add all future paths to the stack of lists recursively
                        KeyValuePair<float, List<Stack<Node>>> keyValue = GetAllPaths(tempStack, _location, shortestPath, newCurrentPathLength);
                        List<Stack<Node>> allFuturePaths = keyValue.Value;
                        shortestPath = keyValue.Key;

                        for (int k = 0; k < allFuturePaths.Count; k++)
                        {
                            paths.Add(allFuturePaths[k]);
                        }
                        neighbors[i].SetVisited(false);
                    }
                }
            }
        }

        KeyValuePair<float, List<Stack<Node>>> values = new KeyValuePair<float, List<Stack<Node>>>(shortestPath, paths);
        return values;
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
        List<string> allPaths = new List<string>();
        Stack<Node> order = new Stack<Node>();
        float shortestPath = -1;
        float newShortestPath = -1;

        for (int k = 0; k < startingNodes.Length; k++)
        {
            startingNodes[k].SetVisited(true);
            order.Clear();
            order.Push(startingNodes[k]);

            //check if currently in room
            if (startingNodes[k].CheckForLocation(location) == true)
            {
                return new Node[] { startingNodes[k] };
            }

            #region ToBe discarded
            /*
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

                    if (!allPaths.Contains(tempOrder)
            !allPaths.Contains(stringTempOrder) && newNeighbors[i].CheckIfVisited() == false)
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
             */
            #endregion

            Stopwatch st = new Stopwatch();
            st.Start();
            KeyValuePair<float, List<Stack<Node>>> values = GetAllPaths(order, location, newShortestPath, -1);
            List<Stack<Node>> tempGoodPaths = new List<Stack<Node>>(values.Value);
            newShortestPath = values.Key;
            st.Stop();
            UnityEngine.Debug.Log(string.Format("Method took {0} ms to complete", st.ElapsedMilliseconds));

            for (int i = 0; i < tempGoodPaths.Count; i++)
            {
                if (tempGoodPaths != null)
                {
                    goodPaths.Add(tempGoodPaths[i]);
                }
            }
        }

        //get shortest path
        //sift through all node arrays
        for (int i = 0; i < goodPaths.Count; i++)
        {
            float distance = -1;
            Node lastNode = null;

            List<Node> tempStack = new List<Node>(goodPaths[i]);
            //sift through each element of node array
            for (int k = 0; k < tempStack.Count; k++)
            {
                if (lastNode != null)
                {
                    tempStack[k].SetVisited(false);
                    if (k == 0)
                    {
                        distance = Mathf.Abs(Vector3.Distance(tempStack[k].gameObject.transform.position, lastNode.gameObject.transform.position));
                    }
                    else
                    {
                        distance += Mathf.Abs(Vector3.Distance(tempStack[k].gameObject.transform.position, lastNode.gameObject.transform.position));
                    }
                }
                lastNode = tempStack[k];
            }

            if (shortestPath == -1 || distance < shortestPath)
            {
                shortestPath = distance;
                path = tempStack.ToArray();
            }
        }

        return path;
    }
}
