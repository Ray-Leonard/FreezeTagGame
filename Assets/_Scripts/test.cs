using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Renderer renderer;
    [SerializeField] Material freezeMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();

        renderer.material = freezeMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
