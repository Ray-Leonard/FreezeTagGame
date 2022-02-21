using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheoryCode : MonoBehaviour
{
    [SerializeField] GameObject point;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector2 pc = new Vector2(3, 6);
        Vector2 pt = new Vector2(5, 4);
        Vector2 vc = new Vector2(2, 3);
        float t = 0.25f;
        float vm = 3.6f;
        float am = 12.25f;
        float t2t = 0.55f;
        float r = 0.5f;
        float rs = 1.5f;


        //for (int i = 0; i < 1000; i++)
        //{
        //    Vector2 desiredAcceleration = am * (pt - pc).normalized;
        //    Debug.Log("Desired acceleration" + desiredAcceleration);

        //    vc += desiredAcceleration * t;
        //    Debug.Log("Current Velocity: " + vc);
        //    vc = Vector2.ClampMagnitude(vc, vm);
        //    Debug.Log("Desired Velocity after clamp" + vc);

        //    pc += vc * t;
        //    Instantiate(point, new Vector3(pc.x, 0, pc.y), Quaternion.identity);
        //    Debug.Log(pc);
        //}


        //for (int i = 0; i < 5; i++)
        //{
        //    Vector2 desiredVelocity = (pt - pc).normalized * vm;

        //    float distance = (pt - pc).magnitude;
        //    Debug.Log("Distance: " + distance);
        //    if (distance > r)
        //    {
        //        if (desiredVelocity.magnitude > distance / t2t)
        //        {
        //            desiredVelocity = (distance / t2t) * desiredVelocity.normalized;
        //        }
        //        Debug.Log("distance / t2t" + (distance / t2t));
        //    }
        //    else
        //    {
        //        desiredVelocity *= 0;
        //    }

        //    Debug.Log("final Velocity: " + desiredVelocity);

        //    pc += desiredVelocity * t;
        //    Instantiate(point, new Vector3(pc.x, 0, pc.y), Quaternion.identity);
        //    Debug.Log(pc);
        //}


        for (int i = 0; i < 10; i++)
        {
            //find out the desired acceleration use max acceleration
            Vector2 desiredAcc = am * (pt - pc).normalized;
            Debug.Log("Desired Acc: " + desiredAcc);

            Debug.Log("Distance: " + (pt - pc).magnitude);
            //if the character inside the slowdown raduiys(Rs)
            if ((pt - pc).magnitude <= rs)
            {
                Debug.Log("In Slowing range");
                //the speed will repect to the distance to target by using an acceleration to adjust it
                float goalVelocityMagnitude = ((pt - pc).magnitude / rs) * vm;
                Debug.Log("Goal Velocity Magnitude: " + goalVelocityMagnitude);

                Vector2 goalVelocity = goalVelocityMagnitude * (pt - pc).normalized;
                Debug.Log("Goal Velocity: " + goalVelocity);

                desiredAcc = (goalVelocity - vc) / t2t;
                Debug.Log("DesiredAcc: " + desiredAcc);

                desiredAcc = Vector2.ClampMagnitude(desiredAcc, am);
                Debug.Log("Desired Acc After clamp: " + desiredAcc);
            }
            //if the character arrive at the stop radius then it will stop 
            if ((pt - pc).magnitude <= r)
            {
                vc *= 0;
                desiredAcc *= 0;
            }

            //this acceleration will add to desired velocity every delt time
            vc = vc + desiredAcc * t;
            Debug.Log("VC " + vc);


            //if this disired velocity bigger than Max velocity
            if (vc.magnitude > vm)
            {
                //then the Max velocity will be used
                vc = vm * vc.normalized;
            }
            Debug.Log("VC after clamp " + vc);

            pc += vc * t;
            Debug.Log("For steering arrive: Next position P" + (i + 1) + " is " + pc);
            Instantiate(point, new Vector3(pc.x, 0, pc.y), Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
