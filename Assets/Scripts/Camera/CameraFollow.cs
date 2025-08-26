using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    
    private void Start()
    {
        if(target==null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }
    void LateUpdate()
    {
        if (target == null)
        {
            
            target = GameObject.FindWithTag("Player").transform;
            return;
        }
            Vector3 desiredPosition = target.position + offset;

       

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

}
