using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreTutorialManager : MonoBehaviour
{

    private AudioSource _myAudioSource;
    public GameObject _tutorial;
    public GameObject _trilhaMenu;
    private bool _finished = false;
    private bool _started = false;

    
    void Start()
    {
        _myAudioSource = this.gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        
    }

    void Update()
    {
        if (!_myAudioSource.isPlaying && !_finished && _started)
        {
            StartCoroutine(LerpAudioVolume(0.5f));
            _finished=true;
            _tutorial.GetComponent<TutorialManager>().createTutorialPortal();
        }
        
    }

    public void PortalDestroyed(){
        _myAudioSource.Play();
        StartCoroutine(LerpAudioVolume(0.15f));
        _started=true;

    }


    private IEnumerator LerpAudioVolume(float _vol)
    {

        // LERPS SCALE
        float timeElapsed = 0;
        float lerpDuration = 1.95f; 
        float newVol;
        while (timeElapsed < lerpDuration)
        {
            newVol = Mathf.Lerp(_trilhaMenu.GetComponent<AudioSource>().volume, _vol, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            _trilhaMenu.GetComponent<AudioSource>().volume = newVol;
            yield return null;
        }
    }
}
