using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNotifier : MonoBehaviour
{
    void Start()
    {
        var camera = FindObjectOfType<Camera>();
        var cameraFollow = camera.GetComponent<CameraFollow>();
        cameraFollow.SetTarget(transform);
    }
}
