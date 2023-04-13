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
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class MainPortalController : MonoBehaviour
{

    public float _timeToSpawn = 5.0f;
    public Vector3 _initialScale = new Vector3(18f,18f,18f);
    public GameObject _mainPortal;
    public GameObject _nextPortal;
    public bool _autoStart=false;

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

    private IEnumerator _cr_portalFxOpen, _cr_portalFxClose;

    private float _portalDissolveMin=0.15f;
    private float _portalDissolveMax=4f;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        _myVideoPlayer = GetComponent<VideoPlayer>();
        _portalFX = this.gameObject.transform.GetChild(0).gameObject;
        _portalParticle = this.gameObject.transform.GetChild(1).gameObject;
        _portalParticle.SetActive(false);
        
        _myVideoPlayer.Pause();
        _myVideoPlayer.loopPointReached += EndReached;

        transform.localScale = new Vector3(0f,0f,0f);

        _cr_portalFxOpen = PortalFXOpenLerp(0);
        _cr_portalFxClose = PortalFXCloseLerp(0);
        

        if (_autoStart)
            StartCoroutine(WaitToSpawnPortal());


        // SetFocusedPortal(true);
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        StartCoroutine(PortalCloseLerp());
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        SetFocusedPortal(true);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        SetFocusedPortal(false);
    }

    /// <summary>
    /// This method spawns the Portal scale
    /// </summary>
    private IEnumerator WaitToSpawnPortal()
    {
        yield return new WaitForSeconds(_timeToSpawn);
        StartCoroutine(SpawnPortal());
    }

    /// <summary>
    /// This method spawns the Portal scale
    /// </summary>
    private IEnumerator SpawnPortal()
    {
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
    }

    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    private void SetFocusedPortal(bool gazedAt)
    {
        if(gazedAt){
            StopCoroutine(_cr_portalFxClose);
            StartCoroutine(_cr_portalFxOpen);
        
        }
        else{
            StopCoroutine(_cr_portalFxOpen);
            StartCoroutine(_cr_portalFxClose);
        }
    }

    /// <summary>
    /// This method opens the portal and starts playing the video
    /// </summary>
    private IEnumerator PortalOpenLerp()
    {
        GameObject tit = GameObject.Find("Título");
        if (tit)
            tit.SetActive(false);


        if(_mainPortal){
            _mainPortal.GetComponent<VideoPlayer>().Pause();
        }

        float timeElapsed = 0;
        float lerpDuration = 3; 
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
        _myVideoPlayer.Play();
        transform.position = new Vector3(0f,0f,0f);
        transform.localScale = _maxScale;

        if(_nextPortal)
            StartCoroutine(_nextPortal.GetComponent<MainPortalController>().WaitToSpawnPortal());
    }

    /// <summary>
    /// This method opens the portal and starts playing the video
    /// </summary>
    private IEnumerator PortalCloseLerp()
    {

        if(_mainPortal){
            _mainPortal.GetComponent<VideoPlayer>().Play();
        }

        float timeElapsed = 0;
        float lerpDuration = 1; 
        Vector3 scaleToLerp = _maxScale;
        while (timeElapsed < lerpDuration)
        {
            scaleToLerp = Vector3.Lerp(transform.localScale, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.localScale = scaleToLerp;
            yield return null;
        }
        transform.localScale = new Vector3(0f,0f,0f);

        if(_nextPortal)
            StartCoroutine(_nextPortal.GetComponent<MainPortalController>().WaitToSpawnPortal());

        Destroy(gameObject);
    }

    private IEnumerator PortalFXOpenLerp(float _t)
    {
        float timeElapsed = _t;
        float lerpDuration = 5; 
        float valueToLerp;
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(_portalDissolveMin, _portalDissolveMax, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            _portalFX.GetComponent<Renderer>().material.SetFloat("_DissolveAmount",valueToLerp);
            yield return null;
        }
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
