using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PortalActivator : MonoBehaviour
{
    private VideoPlayer _myVideoPlayer;
    public GameObject _titulo;

   [System.Serializable]
   public class PortalTimeEntry
   {
       public GameObject _portal;
       public int _timeToSpawn;
       [SerializeField, HideInInspector] public  bool prepared=false;
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
            _titulo.SetActive(false);
            foreach(PortalTimeEntry pt in _portalAndTimes){
                if(!pt.prepared && ((pt._timeToSpawn-30)<_myVideoPlayer.time)){
                    print("SpawnPortal!");
                    pt._portal.GetComponent<MainPortalController>().PrepareVideo();
                    pt.prepared=true;
                }
                if(!pt.played && (pt._timeToSpawn<_myVideoPlayer.time)){
                    print("SpawnPortal!");
                    StartCoroutine(pt._portal.GetComponent<MainPortalController>().SpawnPortal());
                    pt.played=true;
                }
            }
        }
        else{
            _titulo.SetActive(true);
        }
    }
}
