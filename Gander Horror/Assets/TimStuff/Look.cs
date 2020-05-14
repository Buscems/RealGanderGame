using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{



    public float sensitivity;

    public Transform body;

    float Xrotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        Xrotation -= mouseY;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);

        body.Rotate(Vector3.up * mouseX);
    }
}
