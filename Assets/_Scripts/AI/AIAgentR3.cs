using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentR3 : MonoBehaviour
{
    public enum Behavior { Pursue, Flee, Wander, Still }; // different types of AI behaviors

    public Transform target;                            // the target transform 
    public Behavior behavior;                           // this agent's current behavior
    public Vector3 velocity;                            // agent's current velocity
    private float rotationAngle;                         // agent's angle to rotate
    public float maxVelocity = 2.5f;                      // agent's max velocity

    // hyperparameters
    private float maxVelocity_chaser = 5f;
    private float maxVelocity_nonChaser = 2.5f;
    private float maxDegreesDelta = 180f;

    // components
    private Animator animator;                          // model's animator that controls the animation
    private Flee flee;
    private VelocityOrientationAlign lookWhereYouAreGoing;
    private Wander wander;
    private Pursue pursue;
    private Evade evade;

    [SerializeField] AudioSource gotcha;
    [SerializeField] AudioSource thankyou;


    void Start()
    {
        animator = GetComponent<Animator>();
        flee = GetComponent<Flee>();
        lookWhereYouAreGoing = GetComponent<VelocityOrientationAlign>();
        wander = GetComponent<Wander>();
        pursue = GetComponent<Pursue>();
        evade = GetComponent<Evade>();
    }


    void Update()
    {
        if(gameObject.tag == "Chaser")
        {
            maxVelocity = maxVelocity_chaser;
        }
        else
        {
            maxVelocity = maxVelocity_nonChaser;
        }

        switch (behavior)
        {
            case Behavior.Pursue:
                velocity = pursue.GetVelocity(this, target.gameObject.GetComponent<AIAgentR3>());
                break;

            case Behavior.Flee:
                velocity = flee.GetVelocity(this, target);
                //velocity = evade.GetVelocity(this, target.gameObject.GetComponent<AIAgentR3>());
                break;

            case Behavior.Wander:
                velocity = wander.GetVelocity(this);
                break;

            case Behavior.Still:
                velocity *= 0;
                break;
        }

        // update the position and orientation
        transform.position += velocity * Time.deltaTime;
        AlignRotation();

        SetAnimation();
    }



    private void AlignRotation()
    {
        rotationAngle = lookWhereYouAreGoing.GetRotation(this);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                      transform.rotation * Quaternion.Euler(0, rotationAngle, 0), 
                                                      maxDegreesDelta * Time.deltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // IF: I am chaser and I touched the chased one
        // THEN: tag the Chased one to Freeze, and chase the next target.
        // Also if no target is available anymore (returns null from GetNextTarget()), start next game
        if (gameObject.tag == "Chaser" && collision.gameObject.tag == "Chased")
        {
            
            collision.gameObject.tag = "Freeze";
            Transform newTarget = GetComponentInParent<FreezeTagLogicControl>().GetNextTarget();
            if (newTarget == null)
            {
                // if no target available, start a new game with the new chaser being the last being freezed.
                GetComponentInParent<FreezeTagLogicControl>().NextGame(collision.gameObject.GetComponent<AIAgentR3>());
            }
            else
            {
                target = newTarget;
            }
            gotcha.Play();
        }

        // IF: I am Nontag and I touched the Freezed one
        // THEN: tag the Freeze one to nontag.
        if (gameObject.tag == "Nontag" && collision.gameObject.tag == "Freeze")
        {
            collision.gameObject.tag = "Nontag";
            GetComponentInParent<FreezeTagLogicControl>().saveDict.Remove(collision.gameObject.GetComponent<AIAgentR3>());
            thankyou.Play();
        }

    }

    public void ResetWanderParam()
    {
        wander.ResetParam();
    }

    private void SetAnimation()
    {
        animator.SetBool("walking", velocity.magnitude > 0);
        animator.SetBool("running", velocity.magnitude > maxVelocity / 2);
    }
}
