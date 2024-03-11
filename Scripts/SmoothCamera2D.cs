using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera2D : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    public Camera cam;
    private bool is_cam_follow = false;

    void Update()
    {
        if (target && is_cam_follow)
        {
            Vector3 point = Camera.main.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, point.y, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }

    public void set_cam_follow(bool is_act)
    {
        this.is_cam_follow = is_act;
    }

    public void set_model_one_play()
    {
        this.cam.orthographicSize = 5;
    }

    public void set_model_two_play()
    {
        this.cam.orthographicSize = 6f;
    }
}
