using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnSpot : MonoBehaviour
{
    float epslon = 1.0f;    // if difference is within 1 degree, we consider equal
    // this method is to check if the agent is facing the target
    public bool IsFacing(Transform target)
    {
        // angle between character's rotation and the rotation of target-character
        //float offset = Quaternion.Angle(Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up)), 
        //    Quaternion.LookRotation(target.position - transform.position).normalized);
        float targetAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up), 
            (target.position - transform.position).normalized, Vector3.up);
        if (Mathf.Abs(targetAngle) < epslon)
        {
            return true;
        }
        return false;
    }

    public bool IsFacingReverse(Transform target)
    {
        float targetAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up),
            (transform.position - target.position).normalized, Vector3.up);
        Debug.Log(targetAngle);
        if (Mathf.Abs(targetAngle) < epslon)
        {
            return true;
        }
        return false;
    }

    public bool IsTargetInArc(Transform target, float arcAngle)
    {
        float targetAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                (target.position - transform.position).normalized, Vector3.up);

        if(Mathf.Abs(targetAngle) < arcAngle / 2)
        {
            return true;
        }

        return false;
    }

    // return the desired rotation
    public float GetRotation(Transform target)
    {
        Vector3 from = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 to = (target.position - transform.position).normalized;
        float angle = Vector3.SignedAngle(from, to, Vector3.up);

        return angle;
    }

    public float GetRotationReverse(Transform target)
    {
        Vector3 from = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 to = (transform.position - target.position).normalized;
        float angle = Vector3.SignedAngle(from, to, Vector3.up);

        return angle;
    }
}
