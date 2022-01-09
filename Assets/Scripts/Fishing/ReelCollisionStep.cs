using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelCollisionStep : MonoBehaviour
{
    public FishingController fishingController;
   public int StepIndex = 0;

    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log(StepIndex);
        //call fishing controller reelstep
        fishingController.Reel(StepIndex);
    }
}
