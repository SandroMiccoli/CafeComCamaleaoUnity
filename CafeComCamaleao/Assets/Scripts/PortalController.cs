using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{

    public float rotationSpeed = 0f;

    private Vector3 originalPos;
    public float amplitude = 1.5f;
    public float floatingSpeed = 5f;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(0,-Time.deltaTime*rotationSpeed,0,Space.Self);
        Vector3 pos = originalPos;
        pos.y = originalPos.y+amplitude*Mathf.Sin(floatingSpeed*Time.time);
        transform.position = pos;
        
    }

    public void OnPointerEnter(){
        Debug.Log("ENTER");
    }

    public void OnPointerExit(){
        Debug.Log("EXIT");    
    }

    public void OnPointerClick(){
        Debug.Log("CLICK");
    }
}
