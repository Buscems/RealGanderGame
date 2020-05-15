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

    /*
    public static Node[] GetQuickestPathToLocation(Transform actor, Node startingNode, Node.Locations location)
    {
        Node[] path;

        Stack<Node> order = new Stack<Node>();
        startingNode.SetVisited(true);
        order.Push(startingNode);
        float shortestPath;

        while (order.Peek().CheckForLocation(location) != true || order.Count == 0)
        {
            Node newNode = null;
            Node[] newNeighbors = order.Peek().neighbors;

            for(int i = 0; i < newNeighbors.Length; i++)
            {
                if(newNeighbors[i].CheckIfVisited() == false)
                {
                    newNode = newNeighbors[i];
                    newNeighbors[i].SetVisited(true);
                    break;
                }
            }

            if(newNode != null)
            {
                order.Push(newNode);
            }
        }




        return path;
    }
    */
}
