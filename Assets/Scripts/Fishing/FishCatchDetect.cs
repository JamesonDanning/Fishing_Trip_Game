using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCatchDetect : MonoBehaviour
{

    public GameObject player;
    private GameObject caughtFish;
    
    
    private void Awake() 
    {

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("collided");
        if(other.gameObject.tag == "terrain")
        {
             Debug.Log("You caught the fish!");
            //base.ActionInit();
            caughtFish = GameObject.FindGameObjectWithTag("caughtFish");
            player.GetComponent<FishingController>().ActionInit();
        
           
        }
    }
}
