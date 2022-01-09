using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingController : MonoBehaviour
{
    public LineCast lineCast;
    
    public int minFishingDistance;            // Min distance (7ish)
    public int maxFishingDistance;            // Max distance (28ish) 
    public bool wasInterrupted;                // Interrupted flag (moved, mob attacked, etc); public for outside access
   

    private RaycastHit hit;
    private GameObject scriptTarget;        // Create primitive sphere for scriptTarget, replace with fish if hit
    private GameObject fish;                // Create primitive "fish" (replace with model if you have one)
    private GameObject setHook;                // Sphere appears to click
    public GameObject bobberPrefab;
    public GameObject reelUICanvas;

    
    private Vector3 startPos;                // Hold player starting position on cast
    public Vector2 mousePos;

    public ParticleSystem splashParticles;



    private bool lineOut, fishOn, noNibble, hookSet, fishBiting;     // Simple states
    


    //input actions
    public InputAction action;


    private void Awake()
    {
        lineCast = gameObject.GetComponent<LineCast>();

        wasInterrupted = false;
        scriptTarget = null;
        lineOut = false;

        reelUICanvas.SetActive(false);

        

        // Set some defaults if unset
        if (maxFishingDistance <= 0)
            maxFishingDistance = 28;

        if (minFishingDistance < 0)
            minFishingDistance = 7;

        
        
    }




    void Update()
    {
        //reel in, 
        // if (Input.GetKey(KeyCode.Space) && lineOut)
        // {
        //     //Debug.Log("reeling");
        //     //ReelIn();
        //     float step =  5f * Time.deltaTime; // calculate distance to move
        //     //this is so long please fix, moves the bobber towards player but at water height
        //     scriptTarget.transform.position = Vector3.MoveTowards(scriptTarget.transform.position, 
        //         new Vector3(gameObject.transform.position.x, scriptTarget.transform.position.y, gameObject.transform.position.z), step);

        // }

    }




    //*********** Input Actions ***************

    private void OnAction() //button pushed E
    {
        if(lineOut)
        {
            Debug.Log("try setting hook");
            hookSet = true;
        }
    }




    private void OnCast(InputValue value) //right click currently
    {
        float val = value.Get<float>();
        if(val == 1)
        {
            lineCast.lineVisual.enabled = true;
            if(lineOut == false) 
            {
                Debug.Log("start fishing");
                ActionListener();
            }
        }
        
    }




    private void OnMousePos(InputValue value)
    {
        mousePos = value.Get<Vector2>();
    }

    //*********** End Input Actions ***************



   
    private int step = 0;

    public void Reel(int stepIndex)
    {
      if(lineOut)
      {
        if(stepIndex > step)
        {
            step++;
            ReelAction();
        } 
        if(step == 4) step = 0;
      }
    }

    public void ReelAction()
    {
        Debug.Log("reel");
        //move hook towards player small amount
        //tween scriptobject position, make look smooth
         float step =  8f * Time.deltaTime; // calculate distance to move
        //this is so long please fix, moves the bobber towards player but at water height
        scriptTarget.transform.position = Vector3.MoveTowards(scriptTarget.transform.position, 
            new Vector3(gameObject.transform.position.x, scriptTarget.transform.position.y, gameObject.transform.position.z), step);
        
    }




    public void ActionListener()
    {

        ActionInit();
        //ActionChecks();

        //Raycast mouse position looking for Water
        Ray _ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(_ray, out hit, Mathf.Infinity))
        {
            ActionChecks();
        }
        else
        {
            Debug.Log("Aim for the water");
            return;
        }
    }



    public void ActionInit()
    {
        
        // [Re]set states
        wasInterrupted = false;
        lineOut = false;
        fishOn = false;
        noNibble = false;
        hookSet = false;
        fishBiting = false;
        
        //scriptTarget.tag = "Untagged";
       
        //reel in if line is out
        //probably need a coroutine for that
        StopAllCoroutines();
        
        if (scriptTarget != null)
        {
            Destroy(scriptTarget.gameObject);
        }

    }




    private void ActionChecks()
    {
        // Did we hit fishable Water with Raycast? This is determined by simple bool on supplemental 
        // script QM_WaterProps; you could do something different of course. The idea is that our player should not
        // be able to fish everywhere there's water (i.e perhaps an indoor scene, sacred aqueduct, etc)

            // if (hit.transform.GetComponent<canBeFished>() == null || hit.transform.GetComponent<canBeFished>().fishable == false)
            // {
            //     return;
            // }

        // Inventory check? Up to you if you want to check inventory now/return if it's full
        // or check at the very end; either way actual Inventory is out of scope for all of this

        // Check Distance
        float _dist = Vector3.Distance(transform.position, hit.point);

        if (_dist < minFishingDistance)
        {
            Debug.Log("You scare away the fish; try casting further away.");
            return;
        }

        if (_dist > maxFishingDistance)
        {
            Debug.Log("Out of range.");
            return;
        }

        lineOut = true;

        // Demo create Primitive object instead of using a prefab
        scriptTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;


        
        
        
        // Set "bobber" properties
        scriptTarget.transform.position = hit.point;
        scriptTarget.GetComponent<Renderer>().material.color = Color.red;
        scriptTarget.transform.localScale = new Vector3(.25f, .25f, .25f);


        StartCoroutine(ActionMain());
    }




    private IEnumerator ActionMain()
    {

        // Scale according to your taste and WaitForSeconds value
        int fishingTimer = 1000;
        int thisRun = fishingTimer;
       

        // Randoms
       

        // Random 0...1; noNibble means dead cast (but still cycles). You could adjust hard-coded ".2"
        // based on playerSkill, bait/gear quality, buff, etc
        float noBites = Random.value;
        if (noBites <= 0.1f)
        {
            noNibble = true;
        }
        // Range (within fishingTimer) that fish will bite
        // Leave enough room for cycle to complete on low
        int biteAt = Random.Range(500, 800);
        int nibbleNum = Random.Range(1,5);

        int nibbleDelays = biteAt/nibbleNum;
        //376 / 3 == 125; first nibble at 125, 2nd 250, 376 is bite

       
        Vector3 startPos = transform.position;

        // ------------- MAIN ----------------

        while (!wasInterrupted && thisRun > 0)
        {
            if (!fishOn)
            {
                // Demo using Mathf.PingPong to achieve a little bounce to "bobber"
                float scrTgtY = Mathf.PingPong(Time.time/24, 0.016f) - 0.008f;
                scriptTarget.transform.position = new Vector3(scriptTarget.transform.position.x, scriptTarget.transform.position.y + scrTgtY, scriptTarget.transform.position.z);
            }


            if(thisRun % nibbleDelays == 0 && !noNibble)
            {
                Debug.Log("nibble");
            }
            // When thisRun equals Random biteAt, "Fish On!" (unless it's a dead cycle)
            if (!noNibble && thisRun == biteAt)
            {
                //FishOn();
                 StartCoroutine(FishTiming());
            }

            if(hookSet && !fishOn)
            {
                Debug.Log("Too early, you scared them!");
                ActionInit();
            }

          
            // Decrement counter
            thisRun--;

            // If you change Wait len, remember to scale other values
            yield return new WaitForSeconds(0.01f);
        }

        // ------------- end WHILE ----------------

        if (noNibble)
        {
            Debug.Log("The fish aren't biting");
            ActionInit();
            
        }

        // Reset states for next run
        //ActionInit();

        // The End.
        Debug.Log("ActionMain over");
    }


    //Starts the window of reaction for the player to set the hook
    IEnumerator FishTiming()
    {
        fishBiting = true;
        //bobber pulled under water
        scriptTarget.transform.position = new Vector3(scriptTarget.transform.position.x, scriptTarget.transform.position.y - 0.7f, scriptTarget.transform.position.z);
       
        

        float duration = 1f;
        float totalTime = 0;
        Debug.Log("Bite!");
        while (totalTime <= duration)
        {
           
            totalTime += Time.deltaTime;
            //var integer = (int)totalTime; /* choose how to quantize this */
            /* convert integer to string and assign to text */
            if(hookSet) //fix
            {
                SetHook();
                yield break;
            }

            yield return null;
        }

        //missed the fish
        Debug.Log("You missed the damn fish");
        ActionInit();
    }






    //player has successfully set the hook
    private void SetHook()
    {
        fishOn = true;
        Debug.Log("Fish on!");
        reelUICanvas.SetActive(true);
        ParticleSystem splash = Instantiate(splashParticles, new Vector3(scriptTarget.transform.position.x, scriptTarget.transform.position.y,scriptTarget.transform.position.z), Quaternion.identity);
        splash.transform.parent = scriptTarget.transform;
        splash.transform.localScale = new Vector3(1,1,1);
        splash.transform.rotation = Quaternion.Euler(-90,0,0);
        splash.Play();
        //ActionInit();
        //FishOn();
    }




    //Fish is now on hook, start minigame?
    private void FishOn()
    {
    }

}
