using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{

    [SerializeField] private float fleeStopDistance = 10f;

    public Vector3 GetVelocity(AIAgent agent, Transform target)
    {
        Vector3 velocity = agent.transform.position - target.position;
        float distacne = velocity.magnitude;
        velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        velocity = velocity.normalized * agent.maxVelocity;

        if(distacne > fleeStopDistance)
        {
            velocity *= 0;
        }

        return velocity;
    }

    // overload for R3
    public Vector3 GetVelocity(AIAgentR3 agent, Transform target)
    {
        Vector3 velocity = agent.transform.position - target.position;
        float distacne = velocity.magnitude;
        velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        velocity = velocity.normalized * agent.maxVelocity;

        if (distacne > fleeStopDistance)
        {
            velocity *= 0;
        }

        return velocity;
    }
}
