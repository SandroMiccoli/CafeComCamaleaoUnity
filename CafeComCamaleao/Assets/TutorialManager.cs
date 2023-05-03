using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject _mainPortal;
    public GameObject _tutorialPortal;
    public GameObject _titulo;
    private GameObject _tituloParticle;
    private ParticleSystem _tituloParticleSystem;
    private GameObject _tituloSoundtrack;
    private int _tutorialPoints=0;
    private int _tutorialMaxPoints = 3;
    private bool activeTutorial = true;



    void Start()
    {
        _tituloParticle = _titulo.transform.GetChild(0).gameObject;
        _tituloParticleSystem = _tituloParticle.GetComponent<ParticleSystem>();
        _tituloSoundtrack= _titulo.transform.GetChild(1).gameObject;
        // createTutorialPortal();
    }

    // Update is called once per frame
    void Update()
    {
        if(_tutorialPoints>=_tutorialMaxPoints && activeTutorial){
            print("Tutorial finished!");
            _mainPortal.GetComponent<MainPortalController>().activatePortal();
            _tituloSoundtrack.GetComponent<TrilhaManager>().DecreasePitchByTime(20.0f);
            var main = _tituloParticleSystem.main;
            main.startLifetime=15f;
            activeTutorial=false;
            Destroy(gameObject,10f);
        }
        
    }

    public void createTutorialPortal(){
        Invoke("SpawnTutorialPortal", 2.0f);   
    }

    private void SpawnTutorialPortal()
    {
        GameObject instance = Instantiate(_tutorialPortal);
        instance.transform.parent = gameObject.transform;
    }

    public void PortalDestroyed(){
        _tutorialPoints++;
        if(_tutorialPoints<_tutorialMaxPoints)
            createTutorialPortal();
    }


}
