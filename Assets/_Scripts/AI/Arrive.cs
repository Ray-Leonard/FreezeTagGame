using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : MonoBehaviour
{
    [SerializeField] private float t2t = 0.5f;                   // time to target
    [SerializeField] private float stopRadius = 0.5f;            // default stop radius is 0.5
    public Vector3 GetVelocity(AIAgent agent, Transform target)
    {

        //Vector3 targetPos = GetComponent<Edge>().GetWarppedTarget(target);
        Vector3 targetPos = target.position;


        Vector3 velocity = targetPos - agent.transform.position;
        velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        float distance = velocity.magnitude;
        velocity = velocity.normalized * agent.maxVelocity;

        //if(targetPos.Equals(target.position))

        // get the stop radius based on the target type
        // R2 target might have a different stop radius (for demo purposes)
        // normal AI agent will just use the default stop radius
        if (target.GetComponent<Target>() != null)
        {
            stopRadius = target.GetComponent<Target>().stopRadius;
        }

        // now adujust the velocity based on stop radius and t2t
        if(distance < stopRadius)
        {
            velocity *= 0;
        }
        else
        {
            // use t2t to slow character down
            velocity = Vector3.ClampMagnitude(velocity, distance / t2t);

        }

        return velocity;
    }


}
