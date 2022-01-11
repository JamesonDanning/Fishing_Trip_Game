using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class LineCast : MonoBehaviour
{
    private FishingController fc;

    public Rigidbody projectile;
    public GameObject cursor; //worldspace UI
    public Transform castPoint; //gameobject transform?
    public LayerMask layer;
    public LineRenderer lineVisual;
    public int lineSegment = 10;
    public float flightTime;
    private Camera cam;

    public bool casting = false;
 
    // Start is called before the first frame update
    void Start()
    {
        fc = gameObject.GetComponent<FishingController>();
        cam = Camera.main;
        
        lineVisual.positionCount = lineSegment + 1;
        lineVisual.enabled = false;

    }
 
    // Update is called once per frame
    void Update()
    {
        if(casting)
        {
            LaunchProjectile();
        }
        
    }
 
    void LaunchProjectile()
    {
        Ray _ray = cam.ScreenPointToRay(fc.mousePos);
        RaycastHit hit;
 
        if (Physics.Raycast(_ray, out hit, 100f, layer))
        {
            cursor.SetActive(true);
            cursor.transform.position = hit.point + Vector3.up * 0.1f;
 
            //Vector3 testPoint = new Vector3(20,10,20);
            Vector3 vo = CalculateVelocty(hit.point, castPoint.position, flightTime);
 
            Visualize(vo, cursor.transform.position); //we include the cursor position as the final nodes for the line visual position
 
            //transform.rotation = Quaternion.LookRotation(vo);
 
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Rigidbody obj = Instantiate(projectile, castPoint.position, Quaternion.identity);
            //     obj.velocity = vo;
            // }
        }
    }
 
    //added final position argument to draw the last line node to the actual target
    void Visualize(Vector3 vo, Vector3 finalPos)
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, (i / (float)lineSegment) * flightTime);
            lineVisual.SetPosition(i, pos);
        }
 
        lineVisual.SetPosition(lineSegment, finalPos);
    }
 
    Vector3 CalculateVelocty(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXz = distance;
        distanceXz.y = 0f;
 
        float sY = distance.y;
        float sXz = distanceXz.magnitude;
 
        float Vxz = sXz / time;
        float Vy = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);
 
        Vector3 result = distanceXz.normalized;
        result *= Vxz;
        result.y = Vy;
 
        return result;
    }
 
    Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;
 
        Vector3 result = castPoint.position + vo * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + castPoint.position.y;
 
        result.y = sY;
 
        return result;
    }
}