using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : MonoBehaviour
{
    public float stopRadius = 0.5f;
    [SerializeField] private float movingSpeed = 3f;

    // reference to the transparent sphere of the target (for drawing purposes)
    private Transform transparentSphere;

    // Start is called before the first frame update
    void Start()
    {
        transparentSphere = transform.Find("Radius").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // redraw the stop radius 
        if(transparentSphere != null)
            transparentSphere.localScale = new Vector3(stopRadius * 2, stopRadius * 2, stopRadius * 2);

        // allow the target to be moved using wsad
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 velocity = new Vector3(horizontal, 0, vertical).normalized;

        transform.position += velocity * Time.deltaTime * movingSpeed;
    }
}
