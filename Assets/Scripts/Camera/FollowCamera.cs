using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float lookHeight = 1f;

    private Vector3 velocity;
    private bool snap;

    private void OnEnable() { GameManager.LevelReset += RequestSnap; }
    private void OnDisable() { GameManager.LevelReset -= RequestSnap; }

    // The cat is teleported on restart, so jump the camera there instead of flying across the level
    private void RequestSnap() => snap = true;

    private void LateUpdate()
    {
        Vector3 desired = target.position + offset;

        if (snap)
        {
            transform.position = desired;
            velocity = Vector3.zero;
            snap = false;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}