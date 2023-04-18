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

public class TutorialPortalController : MonoBehaviour
{
    private GameObject _portalFX;
    private GameObject _portalSoundFX;

    private GameObject _tutorialManager;

    public Material[] _randomMaterials;

    public void Start()
    {
        _portalFX = this.gameObject.transform.GetChild(0).gameObject;
        _portalSoundFX = this.gameObject.transform.GetChild(1).gameObject;

        _tutorialManager = gameObject.transform.parent.gameObject;

        gameObject.GetComponent<Renderer>().material = _randomMaterials[Random.Range(0, _randomMaterials.Length)];

        StartCoroutine(SpawnPortal());
    }

    public void Update(){
        Vector3 rotationToAdd = new Vector3(0, 0.1f, 0);
        transform.Rotate(rotationToAdd);
    }

    public void OnPointerEnter()
    {
        _portalSoundFX.GetComponent<AudioSource>().Play(0);
        StartCoroutine(PortalCloseLerp());
    }

    public void OnPointerExit()
    {
    }

    public IEnumerator SpawnPortal()
    {
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

    private IEnumerator PortalCloseLerp()
    {

        // LERPS SCALE
        float timeElapsed = 0;
        float lerpDuration = 1; 
        Vector3 scaleToLerp = new Vector3(0f,0f,0f);
        while (timeElapsed < lerpDuration)
        {
            scaleToLerp = Vector3.Lerp(transform.localScale, new Vector3(0f,0f,0f), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.localScale = scaleToLerp;
            yield return null;
        }

        _tutorialManager.GetComponent<TutorialManager>().PortalDestroyed();

        Destroy(gameObject);
    }

}