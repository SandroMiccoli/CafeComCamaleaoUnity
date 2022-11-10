using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
 public GameObject rockPrefab;
 

 void Start()
 {
     StartCoroutine(SpawnRocks());
     Debug.Log(transform.localScale.x);
 }

 IEnumerator SpawnRocks()
 {
     while(true)
     {
         float randomTime = Random.Range(0.02f,0.5f);
         Vector3 randomPosition = new Vector3(Random.Range(-10f,10f),Random.Range(-10f,10f),Random.Range(-10f,10f));
         
         yield return new WaitForSeconds(randomTime);
         Instantiate(rockPrefab,new Vector3(randomPosition.x,randomPosition.y,randomPosition.z),Quaternion.identity);
     }
 }
 
}