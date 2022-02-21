using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : MonoBehaviour
{
    [SerializeField] private float aheadFactor;

    // the function signature takes in AIAgentR3 as a target(unlike Flee we just need a transform to be target),
    // because we need to know the velocity 
    public Vector3 GetVelocity(AIAgentR3 agent, AIAgentR3 target)
    {
        // first check if the target needs to change due to warpping
        Vector3 targetPos = GetComponent<Edge>().GetWarppedTarget(target.transform);

        float distance = Vector3.Distance(targetPos, agent.transform.position);
        float ahead = distance / aheadFactor;

        Vector3 futurePosition = targetPos;
        // do another check: if it does not need warpping to get to the target, then it add the target velocity to it
        if (targetPos.Equals(target.transform.position))
        {
            futurePosition += target.velocity * ahead;
        }

        // now that we get the future position, use kinematic seek to output the goal velocity.
        Vector3 desiredVelocity = futurePosition - agent.transform.position;
        desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, Vector3.up);
        desiredVelocity = desiredVelocity.normalized * agent.maxVelocity;

        return desiredVelocity;
    }
}
