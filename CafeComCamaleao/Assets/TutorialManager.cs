using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject _mainPortal;
    public GameObject _tutorialPortal;
    private int _tutorialPoints=0;
    private int _tutorialMaxPoints = 3;
    private bool activeTutorial = true;



    void Start()
    {
        createTutorialPortal();
    }

    // Update is called once per frame
    void Update()
    {
        if(_tutorialPoints>=_tutorialMaxPoints && activeTutorial){
            print("Tutorial finished!");
            _mainPortal.GetComponent<MainPortalController>().activatePortal();
            activeTutorial=false;
            Destroy(gameObject,10f);
        }
        
    }

    public void createTutorialPortal(){
        Invoke("SpawnTutorialPortal", 2.0f);   
    }

    void SpawnTutorialPortal()
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
