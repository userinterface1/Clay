using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Hand : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private float followSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position, Time.deltaTime * followSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, follow.rotation, Time.deltaTime * followSpeed);
    }
}
