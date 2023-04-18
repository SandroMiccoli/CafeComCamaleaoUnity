using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrilhaManager : MonoBehaviour
{
    private AudioSource _myAudioSource;

    void Start()
    {
        _myAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecreasePitchByTime(float _t){
        StartCoroutine(DecreatePitch(_t));
    }

    private IEnumerator DecreatePitch(float _t){
        float timeElapsed = 0;
        float lerpDuration = _t; 
        float pitchToLerp=0f;
        while (timeElapsed < lerpDuration)
        {
            pitchToLerp = Mathf.Lerp(_myAudioSource.pitch, 0.5f, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            _myAudioSource.pitch=pitchToLerp;
            yield return null;
        }
        _myAudioSource.pitch=pitchToLerp;
    }
}
