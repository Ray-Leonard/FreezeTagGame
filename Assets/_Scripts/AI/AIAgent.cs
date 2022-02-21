using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    public enum Behavior { Arrive, Flee}; // different types of AI behaviors

    public Transform target;                            // the target transform 
    public Behavior behavior;                           // this agent's current behavior
    public Vector3 velocity;             // agent's current velocity
    public float rotationAngle;
    public float maxVelocity = 5f;                      // agent's max velocity

    // hyperparameters
    [SerializeField] private float slowSpeed = 0.5f;    // character is considered moving very slowly if under this speed
    [SerializeField] private float smallDistance = 1f;  // character is considered close to target if under this distance
    [SerializeField] private float maxDegreesDelta = 360f;
    [SerializeField] private float arcDegreeMin = 20f;
    [SerializeField] private float arcDegreeMax = 60f;


    // components
    private Animator animator;                          // model's animator that controls the animation
    private Arrive arrive;
    private Flee flee;
    private TurnOnSpot turnOnSpot;
    private VelocityOrientationAlign lookWhereYouAreGoing;


    void Start()
    {
        animator = GetComponent<Animator>();
        arrive = GetComponent<Arrive>();
        flee = GetComponent<Flee>();
        turnOnSpot = GetComponent<TurnOnSpot>();
        lookWhereYouAreGoing = GetComponent<VelocityOrientationAlign>();
    }


    void Update()
    {
        // update the behavior (fetch a new velocity) every 0.25 second 
        switch (behavior)
        {
            case Behavior.Arrive:
                // the following individual behaviors need to exist before calling the function
                if (arrive != null && turnOnSpot != null && lookWhereYouAreGoing != null)
                    ArriveBehavior();
                break;

            case Behavior.Flee:
                if (flee != null && turnOnSpot != null && lookWhereYouAreGoing != null)
                    FleeBehavior();
                break;
        }

        // update the position and orientation
        transform.position += velocity * Time.deltaTime;

        SetAnimation();
    }

    private void ArriveBehavior()
    {
        // A: If the character is stationary or moving very slow
        if(velocity.magnitude <= slowSpeed)
        {
            // Ai: If it's a small distance from its target, it will step there directly without change in orientation
            if((transform.position - target.position).magnitude <= smallDistance)
            {
                velocity = arrive.GetVelocity(this, target);
            }
            // Aii: Else if the target is far away, character will first turn on spot and then move forward to reach it.
            else if(!turnOnSpot.IsFacing(target))
            {
                // stop moving and interpolate orientation
                velocity *= 0;
                rotationAngle = turnOnSpot.GetRotation(target);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, rotationAngle, 0), maxDegreesDelta * Time.deltaTime);
            }
            else // now its aligned, shoot! 
            {
                velocity = arrive.GetVelocity(this, target);
            }
        }
        // B: Else if the character is moving with some speed, there's a speed dependent arc (faster, arc smaller)
        else
        {
            // calculate the angle of the speed dependant arc
            float arcAngle = Mathf.Lerp(arcDegreeMax, arcDegreeMin, velocity.magnitude / maxVelocity);
            // draw the cone in debug
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(arcAngle / 2, Vector3.up) * transform.forward, Color.cyan);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(-arcAngle / 2, Vector3.up) * transform.forward, Color.cyan);

            // Bi: if target within arc, continue to move forward but add rotational to turn toward the target (velocity orientation align)
            if (turnOnSpot.IsTargetInArc(target, arcAngle))
            {
                velocity = arrive.GetVelocity(this, target);
                // turn toward the target
                rotationAngle = lookWhereYouAreGoing.GetRotation(this);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, rotationAngle, 0), maxDegreesDelta * Time.deltaTime);
            }
            // Bii: target outside arc, stop moving and change direction on spot
            else
            {
                velocity *= 0;
                rotationAngle = turnOnSpot.GetRotation(target);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, rotationAngle, 0), maxDegreesDelta * Time.deltaTime);
            }
        }
    }

    private void FleeBehavior()
    {
        // small distance from target, step away directly
        if ((transform.position - target.position).magnitude <= smallDistance)
        {
            velocity = flee.GetVelocity(this, target);
        }
        // Aii: Else if the target is far away, character will first turn on spot and then move forward to reach it.
        else if (!turnOnSpot.IsFacingReverse(target))
        {
            //Debug.Log("TrunOnSpot");
            // stop moving and interpolate orientation
            velocity *= 0;
            rotationAngle = turnOnSpot.GetRotationReverse(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, rotationAngle, 0), maxDegreesDelta * Time.deltaTime);
        }
        else // now direction is in good condition, shoot! 
        {
            //Debug.Log("Fleeing");

            velocity = flee.GetVelocity(this, target);
            // align orientation with velocity direction
            rotationAngle = lookWhereYouAreGoing.GetRotation(this);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, rotationAngle, 0), maxDegreesDelta * Time.deltaTime);
        }
    }



    private void SetAnimation()
    {
        animator.SetBool("walking", velocity.magnitude > 0);
        animator.SetBool("running", velocity.magnitude > maxVelocity / 2);
    }
}
