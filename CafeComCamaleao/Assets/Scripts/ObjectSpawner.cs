using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
 public GameObject rockPrefab;
 // Start is called before the first frame update
 void Start()
 {
     StartCoroutine(SpawnRocks());
 }

 IEnumerator SpawnRocks()
 {
     while(true)
     {
         float randomTime = Random.Range(0.2f,1.5f);
         float randomPosition = Random.Range(-10f,10f);
         
         yield return new WaitForSeconds(randomTime);
         Instantiate(rockPrefab,new Vector3(randomPosition,transform.position.y,transform.position.z),Quaternion.identity);
     }
 }
 
}