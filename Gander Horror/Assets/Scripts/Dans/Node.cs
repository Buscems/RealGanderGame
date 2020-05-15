using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum Locations { Kitchen, Bathroom, Bedroom, Living_Room };

    public Node[] neighbors;
    public Locations[] locations;

    [Tooltip("When Goose goes to this nodes location, it will choose a random point near this postion based on the radius; Make sure radius doesn't go through walls. Y pos will not be affected by radius.")]
    [SerializeField]
    [Range(0, 5)]
    private float radius;
    //draw wireframe so user can see the radius in the editor real-time
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private bool visited = false;
#endif

    //To be updated as new areas are added
    private Node[] savedPath_Kitchen = null;
    private Node[] savedPath_Bathroom = null;
    private Node[] savedPath_Bedroom = null;
    private Node[] savedPath_LivingRoom = null;


    //Give goose new point to go to
    public Vector3 GetDestination()
    {
        Vector3 positionToGoTo = transform.position;
        positionToGoTo.x = positionToGoTo.x + Random.Range(-radius, radius);
        positionToGoTo.z = positionToGoTo.z + Random.Range(-radius, radius);

        return positionToGoTo;
    }

    //Get new neighbor
    public Node GetNewNeighbor(Node lastDestination, float chanceForNewNode)
    {
        int randNode = 0;
        if (neighbors.Length > 1 && lastDestination != null)
        {
            int randNum = Random.Range(0, 100);

            if (randNum < chanceForNewNode)
            {
                List<Node> neighborsExcludingLastDestination = new List<Node>(neighbors);
                if (neighborsExcludingLastDestination.Contains(lastDestination))
                {
                    neighborsExcludingLastDestination.Remove(lastDestination);
                }
                randNode = Random.Range(0, neighborsExcludingLastDestination.ToArray().Length);
                return neighborsExcludingLastDestination.ToArray()[randNode];
            }
            else
            {
                return neighbors[randNode];
            }
        }
        randNode = Random.Range(0, neighbors.Length);
        return neighbors[randNode];
    }

    //return visitation check; For when determining the shortest path to take via the WaypointNavigation script
    public bool CheckIfVisited()
    {
        return visited;
    }

    //set the visitation status
    public void SetVisited(bool status)
    {
        visited = status;
    }

    //returns true if this node is in the location the Goose is looking for
    public bool CheckForLocation(Locations _location)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].Equals(_location))
            {
                return true;
            }
        }
        return false;
    }

    //this will be removed if we determine all paths at the beginning of the game
    #region savedPath functions

    public Node[] CheckForPathToKitchen()
    {
        if (savedPath_Kitchen != null)
        {
            return savedPath_Kitchen;
        }
        return null;
    }

    public void SetPathToKitchen(Node[] path)
    {
        if (savedPath_Kitchen == null)
        {
            savedPath_Kitchen = path;
        }
    }

    public Node[] CheckForPathToBathroom()
    {
        if (savedPath_Bathroom != null)
        {
            return savedPath_Bathroom;
        }
        return null;
    }

    public void SetPathToBathroom(Node[] path)
    {
        if (savedPath_Bathroom == null)
        {
            savedPath_Bathroom = path;
        }
    }

    public Node[] CheckForPathToBedroom()
    {
        if (savedPath_Bedroom != null)
        {
            return savedPath_Bedroom;
        }
        return null;
    }

    public void SetPathToBedroom(Node[] path)
    {
        if (savedPath_Bedroom == null)
        {
            savedPath_Bedroom = path;
        }
    }

    public Node[] CheckForPathToLivingRoom()
    {
        if (savedPath_LivingRoom != null)
        {
            return savedPath_LivingRoom;
        }
        return null;
    }

    public void SetPathToLivingRoom(Node[] path)
    {
        if (savedPath_LivingRoom == null)
        {
            savedPath_LivingRoom = path;
        }
    }


    #endregion
}
