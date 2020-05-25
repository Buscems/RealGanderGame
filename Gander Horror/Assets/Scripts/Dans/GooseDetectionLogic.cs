using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooseDetectionLogic : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxDetectionDistance;
    [SerializeField] private float minLookDetectionTime;
    [SerializeField] private float maxTimeBonus;
    [SerializeField] private int detectionAngle;

    public GameObject[] playerLimbs;

    private float currentDetectionTime;

    // Start is called before the first frame update
    void Start()
    {
        currentDetectionTime = minLookDetectionTime;
    }

    // Update is called once per frame
    void Update()
    {
        //update detection time depending on distance, line of sight, and viewpoint
        //0  -> player outside of detection
        //minLookDetectionTime  -> player seen
        currentDetectionTime += Time.deltaTime / RaycastDetection();
        currentDetectionTime = Mathf.Clamp(currentDetectionTime, 0, minLookDetectionTime);

    }

    private float RaycastDetection()
    {
        float distance = Vector3.Distance(playerLimbs[1].transform.position, transform.position);
        float angle = Equations.GetAngleBetweenTwoPoints(playerLimbs[0].transform.position, transform);
        if (distance > minMaxDetectionDistance.y || angle > detectionAngle) return -1;
        if (distance < minMaxDetectionDistance.x) return .00001f;

        float speed = 0;

        //check if player is within range
        for (int i = 0; i < playerLimbs.Length; i++)
        {
            RaycastHit hit;
            Vector3 direction = playerLimbs[i].transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if(hit.collider.gameObject.tag == "Player")
                {
                   speed += 1;
                }
            }
        }
        if (speed == 0) return -1;

        //(currentX - minX) / (maxX - minX)
        float percentage = Mathf.Abs((distance - minMaxDetectionDistance.y) / (minMaxDetectionDistance.x - minMaxDetectionDistance.y)); //calculate percentage based on distance
        percentage *= Mathf.Abs((speed - 0) / (playerLimbs.Length - 0)); //change percentage based on how much of the players body is showing
        float time = (minLookDetectionTime) + (maxTimeBonus - (maxTimeBonus * percentage)); //add any extra time to detect based on ^ pecentage

        return (time / minLookDetectionTime);
    }
}
