//-----------------------------------------------------------------------
// <copyright file="PortalController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class MainPortalController : MonoBehaviour
{

    public Material _previewMaterial,_videoMaterial;
    public float _timeToSpawn = 5.0f;
    public Vector3 _initialScale = new Vector3(18f,18f,18f);
    public GameObject _mainPortal;
    public bool _isMainPortal=false;
    public bool _autoStart=false;
    public bool _selfDestroy=false;

    public string _videoFileName;

    public Vector3 _maxScale = new Vector3(-100f,-100f,-100f);

    private const float _minObjectDistance = 2.5f;
    private const float _maxObjectDistance = 3.5f;
    private const float _minObjectHeight = 0.5f;
    private const float _maxObjectHeight = 3.5f;

    private Renderer _myRenderer;
    private Vector3 _startingPosition;
    private VideoPlayer _myVideoPlayer;

    private GameObject _portalFX;
    private GameObject _portalParticle;
    private ParticleSystem _portalParticleSystem;
    private GameObject _portalSoundFX;

    private IEnumerator _cr_portalFxOpen, _cr_portalFxClose;

    private float _portalDissolveMin=-0.72f;
    private float _portalDissolveMax=3f;

    private bool _spawned = false;
    private bool _open = false;

    public void Awake(){
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        _myVideoPlayer = GetComponent<VideoPlayer>();
        _portalFX = this.gameObject.transform.GetChild(0).gameObject;
        _portalParticle = this.gameObject.transform.GetChild(1).gameObject;
        _portalSoundFX = this.gameObject.transform.GetChild(2).gameObject;
        _portalParticle.SetActive(false);
        
        _myVideoPlayer.url = Path.Combine(Application.streamingAssetsPath, _videoFileName);
        _myVideoPlayer.prepareCompleted += VideoPrepared;
        // _myVideoPlayer.Prepare();
        _myVideoPlayer.loopPointReached += EndReached;

        transform.localScale = new Vector3(0f,0f,0f);

        _cr_portalFxOpen = PortalFXOpenLerp(0);
        _cr_portalFxClose = PortalFXCloseLerp(0);

        _portalParticleSystem = _portalParticle.GetComponent<ParticleSystem>();
        

        if (_autoStart)
            StartCoroutine(WaitToSpawnPortal());


        // SetFocusedPortal(true);
    }

    public void Update(){
        // slowly takes from scale, if 0 then destroy portal
        if(_spawned && !_open && _selfDestroy){
            transform.localScale -= new Vector3(0.0075f,0.0075f,0.0075f);
            if(transform.localScale.x<0){
                Destroy(gameObject);
            }
        }

        // if(!_myVideoPlayer.isPrepared){
        //     _myVideoPlayer.Prepare();
        // }


        if(!_isMainPortal && _open && !_myVideoPlayer.isPlaying){
            _myVideoPlayer.Play();
        }
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        StartCoroutine(PortalCloseLerp());
    }

    void VideoPrepared(UnityEngine.Video.VideoPlayer vp) {
        print("Video prepared: "+_videoFileName);
        _myVideoPlayer.Play();
        _myVideoPlayer.Pause();
    }

    public void PrepareVideo(){
        _myVideoPlayer.Prepare();
    }


    //
    // INTERACTION
    //

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        SetFocusedPortal(true);
        _portalSoundFX.GetComponent<AudioSource>().Play(0);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        SetFocusedPortal(false);
        _portalSoundFX.GetComponent<AudioSource>().Pause();
    }

    /// <summary>
    /// This method waits time in seconds before showing the portal
    /// </summary>
    private IEnumerator WaitToSpawnPortal()
    {
        yield return new WaitForSeconds(_timeToSpawn);
        StartCoroutine(SpawnPortal());
    }

    public void activatePortal(){
        StartCoroutine(SpawnPortal());
    }

    /// <summary>
    /// This method spawns the Portal scale
    /// </summary>
    public IEnumerator SpawnPortal()
    {
        gameObject.GetComponent<MeshRenderer>().material = _previewMaterial;
        _portalParticle.SetActive(true);
        float timeElapsed = 0;
        float lerpDuration = 3; 
        Vector3 scaleToLerp = new Vector3(0f,0f,0f);
        while (timeElapsed < lerpDuration)
        {
            scaleToLerp = Vector3.Lerp(new Vector3(0f,0f,0f), _initialScale, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.localScale = scaleToLerp;
            yield return null;
        }
        transform.localScale = _initialScale;
        _spawned = true;
    }

    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    private void SetFocusedPortal(bool gazedAt)
    {
        if(gazedAt && _cr_portalFxClose!=null){
            StopCoroutine(_cr_portalFxClose);
            StartCoroutine(_cr_portalFxOpen);
        
        }
        else{
            StopCoroutine(_cr_portalFxOpen);
            StartCoroutine(_cr_portalFxClose);
        }
    }

    /// <summary>

    // After Open FX finished
    // kill fxs and open and plays video
    /// This method opens the portal and starts playing the video
    /// </summary>
    private IEnumerator PortalOpenLerp()
    {
        // PAUSE MAIN VIDEO
        if(_mainPortal!=null){
            _mainPortal.GetComponent<VideoPlayer>().Pause();
        }

        gameObject.GetComponent<MeshRenderer>().material = _videoMaterial;

        // START THIS PORTAL VIDEO
        _myVideoPlayer.Play();

        // LERPS POSITION AND SCALE
        float timeElapsed = 0;
        float lerpDuration = 5; 
        Vector3 positionToLerp = new Vector3(0f,0f,0f);
        Vector3 scaleToLerp = _maxScale;
        while (timeElapsed < lerpDuration)
        {
            positionToLerp = Vector3.Lerp(transform.position, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            scaleToLerp = Vector3.Lerp(transform.localScale, _maxScale, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.position = positionToLerp;
            transform.localScale = scaleToLerp;
            yield return null;
        }
        transform.position = new Vector3(0f,0f,0f);
        transform.localScale = _maxScale;
        _open=true;

    }

    /// <summary>
    /// This method opens the portal and starts playing the video
    /// </summary>
    private IEnumerator PortalCloseLerp()
    {
        // RESUME MAIN VIDEO
        if(_mainPortal!=null){
            _mainPortal.GetComponent<VideoPlayer>().frame=(long)(_timeToSpawn+2.5f)*30;
            _mainPortal.GetComponent<VideoPlayer>().Play();
        }
        
        // LERPS SCALE
        float timeElapsed = 0;
        float lerpDuration = 2; 
        Vector3 scaleToLerp = _maxScale;
        while (timeElapsed < lerpDuration)
        {
            scaleToLerp = Vector3.Lerp(transform.localScale, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.localScale = scaleToLerp;
            yield return null;
        }
        transform.localScale = new Vector3(0f,0f,0f);

        Destroy(gameObject);
    }

    //
    // FX
    //

    private IEnumerator PortalFXOpenLerp(float _t)
    {
        float timeElapsed = _t;
        float lerpDuration = 5; 
        float emissionRate = 0.0f;
        float emissionRateInc = 10.0f;
        float valueToLerp;
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(_portalDissolveMin, _portalDissolveMax, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            _portalFX.GetComponent<Renderer>().material.SetFloat("_DissolveAmount",valueToLerp);
            
            var _portalParticleSystemEmission = _portalParticleSystem.emission;
            _portalParticleSystemEmission.enabled = true;
            _portalParticleSystemEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f, emissionRate);
            emissionRate+=emissionRateInc;
            transform.localScale += new Vector3(0.01f,0.01f,0.01f);
             
            // or for a funky light heavy effect:
            // _portalParticleSystemEmission.rateOverTime = Mathf.Lerp(particleEmissionMin, particleEmissionMax, startOfEmission/ emissionLength);
 
            yield return null;
        }

        // After Open FX finished
        // kill fxs and open and play ivdoe
        _portalFX.SetActive(false);
        _portalParticle.SetActive(false);
        StartCoroutine(PortalOpenLerp());
    }

    private IEnumerator PortalFXCloseLerp(float _t)
    {
        float timeElapsed = _t;
        float lerpDuration = 1; 
        float valueToLerp;
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(_portalDissolveMax, _portalDissolveMin, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            _portalFX.GetComponent<Renderer>().material.SetFloat("_DissolveAmount",valueToLerp);
            yield return null;
        }
        valueToLerp = _portalDissolveMin;
        _portalFX.GetComponent<Renderer>().material.SetFloat("_DissolveAmount",valueToLerp);
    }
}
