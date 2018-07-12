using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomFunction : MonoBehaviour {

    Camera cam;
    public float minZoom = 10f;
    public float maxZoom = 50f;

    public float zoomSpeed = 5f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update () {
    
        if(Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.W))
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }

        if(Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.S))
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }
	}
}
