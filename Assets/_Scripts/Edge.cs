using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    // plane is a square, we'll make the character appear from the other side
    [SerializeField] private Transform referencePoint;
    private float planeRadius;
    // Start is called before the first frame update
    void Start()
    {
        // use the reference point to know how big the plane is
        planeRadius = referencePoint.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currPos = transform.position;
        // go off the left edge
        if(transform.position.x < -planeRadius)
        {
            currPos.x = planeRadius;
        }
        // go off the right edge
        if(transform.position.x > planeRadius)
        {
            currPos.x = -planeRadius;
        }
        // go off top edge
        if(transform.position.z > planeRadius)
        {
            currPos.z = -planeRadius;
        }
        // go off bottom edge
        if(transform.position.z < -planeRadius)
        {
            currPos.z = planeRadius;
        }

        transform.position = currPos;
    }

    // a function that returns the new target point taking warp into consideration.
    public Vector3 GetWarppedTarget(Transform target)
    {
        Vector3 newTarget = new Vector3(target.position.x, target.position.y, target.position.z);
        
        if (Mathf.Abs(transform.position.x - target.position.x) > planeRadius)
        {
            // set the nearest edge (wrt this agent) in the x direction to be the new target
            float newX = transform.position.x / Mathf.Abs(transform.position.x) * (planeRadius + 0.1f);
            newTarget.x = newX;
            newTarget.z = transform.position.z;
        }
        else if(Mathf.Abs(transform.position.z - target.position.z) > planeRadius)
        {
            float newZ = transform.position.z / Mathf.Abs(transform.position.z) * (planeRadius + 0.1f);
            newTarget.z = newZ;
            newTarget.x = transform.position.x;
        }

        return newTarget;
    }
}
