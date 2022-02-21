using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityOrientationAlign : MonoBehaviour
{
    public float GetRotation(AIAgent agent)
    {
        float angle = 0;
        if (agent.velocity.magnitude > 0)
        {
            Vector3 from = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            Vector3 to = agent.velocity.normalized;
            angle = Vector3.SignedAngle(from, to, Vector3.up);
        }


        return angle;
    }

    // overload for R3
    public float GetRotation(AIAgentR3 agent)
    {
        float angle = 0;
        if (agent.velocity.magnitude > 0)
        {
            Vector3 from = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            Vector3 to = agent.velocity.normalized;
            angle = Vector3.SignedAngle(from, to, Vector3.up);
        }


        return angle;
    }
}
