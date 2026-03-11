using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(camera.transform.position);
        transform.Rotate(Vector3.up * 180f);
    }
}