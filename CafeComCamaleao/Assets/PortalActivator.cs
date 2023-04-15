using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PortalActivator : MonoBehaviour
{
    private VideoPlayer _myVideoPlayer;

   [System.Serializable]
   public class PortalTimeEntry
   {
       public GameObject _portal;
       public int _timeToSpawn;
       [SerializeField, HideInInspector] public  bool played=false;
   }

   public PortalTimeEntry[] _portalAndTimes;    


    void Start()
    {
        _myVideoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        
        if(_myVideoPlayer.isPlaying){
            foreach(PortalTimeEntry pt in _portalAndTimes){
                if(!pt.played && (pt._timeToSpawn<_myVideoPlayer.time)){
                    print("SpawnPortal!");
                    StartCoroutine(pt._portal.GetComponent<MainPortalController>().SpawnPortal());
                    pt.played=true;
                }
            }
        }
    }
}
