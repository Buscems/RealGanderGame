using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;


public class MoveCamera : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = player.transform.position;
    }
}
