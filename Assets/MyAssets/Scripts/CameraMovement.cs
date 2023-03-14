using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Min(0.0001f)]  float maxCameraSpeed;
    [SerializeField] Collider cameraBoundingBox;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0,0,0);

        //Arrow Key Movement
        if (xInput != 0 || yInput != 0)
        {
            direction = new Vector3(xInput, 0.0f, yInput);
        }

        //Mouse movement
        else
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 screenResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
            int screenEdgeMagnitudeX = (int)((float)screenResolution.x * 0.1f);
            int screenEdgeMagnitudeY = (int)((float)screenResolution.y * 0.1f);
            //left
            if (mousePos.x < screenEdgeMagnitudeX)
            {
                direction += Vector3.left;
            }
            //right
            else if(mousePos.x > screenResolution.x - screenEdgeMagnitudeX)
            {
                direction += Vector3.right;
            }
            //down
            if (mousePos.y < screenEdgeMagnitudeY)
            {
                direction += Vector3.back;
            }
            //forward
            else if (mousePos.y > screenResolution.y - screenEdgeMagnitudeY)
            {
                direction += Vector3.forward;
            }
        }
        direction = Vector3.ClampMagnitude(direction.normalized, maxCameraSpeed * Time.deltaTime);

        //Translation and box confine
        transform.Translate(direction);
        transform.position = cameraBoundingBox.ClosestPoint(transform.position);
     
    }
}
