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

public class PreTutorialPortalController : MonoBehaviour
{
    private GameObject _portalFX;
    private GameObject _portalSoundFX;
    private GameObject _portalHoverSoundFX;

    private GameObject _tutorialManager;

    public Material[] _randomMaterials;

    private IEnumerator _cr_portalFxOpen, _cr_portalFxClose;

    private float _portalDissolveMin=-0.72f;
    private float _portalDissolveMax=1f;

    public Vector3 _initialPosition;

    public void Start()
    {
        _portalFX = this.gameObject.transform.GetChild(0).gameObject;
        _portalSoundFX = this.gameObject.transform.GetChild(1).gameObject;
        _portalHoverSoundFX = this.gameObject.transform.GetChild(4).gameObject;

        _tutorialManager = gameObject.transform.parent.gameObject;

        _cr_portalFxClose = PortalFXCloseLerp();

        gameObject.GetComponent<Renderer>().material = _randomMaterials[Random.Range(0, _randomMaterials.Length)];

        StartCoroutine(SpawnPortal());
    }

    public void Update(){
        // Vector3 rotationToAdd = new Vector3(0, 0.05f, 0);
        // transform.Rotate(rotationToAdd);
    }

    public void OnPointerEnter()
    {
        SetFocusedPortal(true);
        _portalHoverSoundFX.GetComponent<AudioSource>().Play();
    }

    public void OnPointerExit()
    {
        SetFocusedPortal(false);
        _portalHoverSoundFX.GetComponent<AudioSource>().Pause();
    }

    private void SetFocusedPortal(bool gazedAt)
    {
        if(gazedAt && _cr_portalFxClose!=null){
            StartCoroutine(_cr_portalFxClose);
        }
        else{
            StopCoroutine(_cr_portalFxClose);
        }
    }

    public IEnumerator SpawnPortal()
    {

        if(_initialPosition==null){
            float timeElapsed = 0;
            float lerpDuration = 3f; 
            Vector3 scaleToLerp = new Vector3(0f,0f,0f);
            Vector3 positionToLerp = new Vector3(0f,0f,0f);
    
            Vector3 finalPosition = new Vector3(Random.Range(-150f,150f),Random.Range(-150f,150f),Random.Range(-150f,150f));
            while(Vector3.Distance(finalPosition,new Vector3(0f,0f,0f))<50f)
                finalPosition = new Vector3(Random.Range(-150f,150f),Random.Range(-150f,150f),Random.Range(-150f,150f));
    
            while (timeElapsed < lerpDuration)
            {
                positionToLerp = Vector3.Lerp(transform.position, finalPosition, timeElapsed / lerpDuration);
                scaleToLerp = Vector3.Lerp(new Vector3(0f,0f,0f), new Vector3(30f,30f,30f), timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                transform.position = positionToLerp;
                transform.localScale = scaleToLerp;
                yield return null;
            }
            transform.localScale = new Vector3(30f,30f,30f);
            transform.position = finalPosition;
        }
        else {
            transform.position = _initialPosition;
        }
    }

    private IEnumerator PortalCloseLerp()
    {

        _portalSoundFX.GetComponent<AudioSource>().Play(0);

        // LERPS SCALE
        float timeElapsed = 0;
        float lerpDuration = 0.2f; 
        Vector3 scaleToLerp = new Vector3(0f,0f,0f);
        while (timeElapsed < lerpDuration)
        {
            scaleToLerp = Vector3.Lerp(transform.localScale, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.localScale = scaleToLerp;
            yield return null;
        }

        _tutorialManager.GetComponent<PreTutorialManager>().PortalDestroyed();

        Destroy(gameObject);
    }

    private IEnumerator PortalFXCloseLerp()
    {
        float timeElapsed = 0;
        float lerpDuration = 4f; 
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

        StartCoroutine(PortalCloseLerp());
    }

}