using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : MonoBehaviour
{
    [SerializeField] private float aheadFactor;

    // the function signature takes in AIAgentR3 as a target(unlike Flee we just need a transform to be target),
    // because we need to know the velocity 
    public Vector3 GetVelocity(AIAgentR3 agent, AIAgentR3 target)
    {
        float distance = Vector3.Distance(target.transform.position, agent.transform.position);
        float ahead = distance / aheadFactor;
        Vector3 futurePosition = target.transform.position + target.velocity * ahead;

        // now that we get the future position, use kinematic seek to output the goal velocity.
        Vector3 desiredVelocity = agent.transform.position - futurePosition;
        desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, Vector3.up);
        desiredVelocity = desiredVelocity.normalized * agent.maxVelocity;

        return desiredVelocity;
    }
}
