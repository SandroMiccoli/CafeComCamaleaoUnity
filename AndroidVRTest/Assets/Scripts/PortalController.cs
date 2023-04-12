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
public class PortalController : MonoBehaviour
{

    // The objects are about 1 meter in radius, so the min/max target distance are
    // set so that the objects are always within the room (which is about 5 meters
    // across).
    private const float _minObjectDistance = 2.5f;
    private const float _maxObjectDistance = 3.5f;
    private const float _minObjectHeight = 0.5f;
    private const float _maxObjectHeight = 3.5f;

    private Renderer _myRenderer;
    private Vector3 _startingPosition;
    private VideoPlayer _myVideoPlayer;

    private GameObject _portalFX;
    private GameObject _portalParticle;

    private float _portalDissolveMin=1.63f;
    private float _portalDissolveMax=15f;
    private const float _PORTAL_GROWTH_SPEED = 8.0f;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        _myVideoPlayer = GetComponent<VideoPlayer>();
        _portalFX = this.gameObject.transform.GetChild(0).gameObject;
        _portalParticle = this.gameObject.transform.GetChild(1).gameObject;
        
        // SetFocusedPortal(true);
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
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnPointerClick()
    {
        OpenPortal();
    }

    /// <summary>
    /// This method opens the portal and starts playing the video
    /// </summary>
    public void OpenPortal()
    {
        
    }

    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    private void SetFocusedPortal(bool gazedAt)
    {
        if(gazedAt){
            StopCoroutine(PortalFXCloseLerp());
            StartCoroutine(PortalFXOpenLerp());
        
        }
        else{
            StopCoroutine(PortalFXOpenLerp());
            StartCoroutine(PortalFXCloseLerp());
        }
    }



    private IEnumerator PortalOpenLerp()
    {
        float timeElapsed = 0;
        float lerpDuration = 5; 
        Vector3 positionToLerp = new Vector3(0f,0f,0f);
        Vector3 scaleToLerp = new Vector3(0f,0f,0f);
        _myVideoPlayer.Play();
        while (timeElapsed < lerpDuration)
        {
            positionToLerp = Vector3.Lerp(transform.position, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            scaleToLerp = Vector3.Lerp(transform.localScale, new Vector3(100f,100f,100f), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.position = positionToLerp;
            transform.localScale = scaleToLerp;
            yield return null;
        }
        positionToLerp = new Vector3(0f,0f,0f);
        transform.position = positionToLerp;
        transform.localScale = new Vector3(100f,100f,100f);

    }

    private IEnumerator PortalFXOpenLerp()
    {
        float timeElapsed = 0;
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

    private IEnumerator PortalFXCloseLerp()
    {
        float timeElapsed = 0;
        float lerpDuration = 3; 
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