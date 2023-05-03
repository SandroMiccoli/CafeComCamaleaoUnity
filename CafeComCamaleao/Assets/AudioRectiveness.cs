using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRectiveness : MonoBehaviour
{

    public GameObject _audioObject;
    private AudioSource _audioSource;

    private float volume;

    private int qSamples = 4096;
    private float[] samples;

    // Start is called before the first frame update
    void Start()
    {
         samples = new float[qSamples];
         _audioSource = _audioObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
         volume = GetRMS(0) + GetRMS(1);
         gameObject.GetComponent<Renderer>().material.SetFloat("_DissolveAmount",volume*-3-0.9f);
    }

    float GetRMS(int channel){
    //Replaced the AudioListener with the public AudioSource _audioSource from above
        _audioSource.GetOutputData(samples, channel);  
        float sum = 0;
        foreach(float f in samples){
            sum += f*f;
        }
        return Mathf.Sqrt(sum/qSamples);
    }
}
