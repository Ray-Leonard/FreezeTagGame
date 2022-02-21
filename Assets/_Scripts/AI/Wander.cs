using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private float wanderTimer = 0f;
    [SerializeField] private float wanderInterval = 0.5f;
    private Vector3 lastWanderDirection = Vector3.zero;
    private Vector3 lastDisplacement = Vector3.zero;
    [SerializeField] private float wanderDegreesDelta = 120f;

    public Vector3 GetVelocity(AIAgentR3 agent)
    {
        wanderTimer += Time.deltaTime;

        if(lastWanderDirection == Vector3.zero)
        {
            lastWanderDirection = agent.transform.forward;
        }

        if(lastDisplacement == Vector3.zero)
        {
            lastDisplacement = agent.transform.forward * agent.maxVelocity;
        }

        Vector3 desiredVelocity = lastDisplacement;
        if(wanderTimer > wanderInterval)
        {
            float angle = (Random.value - Random.value) * wanderDegreesDelta;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * lastWanderDirection.normalized;
            Vector3 circleCenter = agent.transform.position + lastDisplacement;
            Vector3 destination = circleCenter + direction;
            desiredVelocity = destination - agent.transform.position;
            desiredVelocity = desiredVelocity.normalized * agent.maxVelocity;

            lastDisplacement = desiredVelocity;
            lastWanderDirection = direction;
            wanderTimer = 0;
        }

        return desiredVelocity;
    }

    public void ResetParam()
    {
        wanderTimer = 0;
        lastWanderDirection = Vector3.zero;
        lastDisplacement = Vector3.zero;
    }
}
