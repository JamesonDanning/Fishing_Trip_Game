using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleRotation : MonoBehaviour
{
    public float speed = 5f;

    
    // Update is called once per frame
    void Update()
    {
        RectTransform rectRot = GetComponent<RectTransform>();
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        Vector2 direction =  Mouse.current.position.ReadValue() - pos;

        float angle  = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rectRot.rotation = Quaternion.Euler(0f,0f,angle);
        
        
    }
}
