using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 rotationToAdd = new Vector3(0, 0.025f, 0);
        transform.Rotate(rotationToAdd);
        
    }
}
