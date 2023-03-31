using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Min(0.0001f)]  float maxCameraSpeed;
    [SerializeField, Range(0.0f, 0.4f)] float screenEdgeFraction;
    [SerializeField] Collider cameraBoundingBox;

    void Update()
    {
        
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0,0,0);
        Vector3 weight = new Vector3(0,0,0);
        //Arrow Key Movement
        if (xInput != 0 || yInput != 0)
        {
            direction = new Vector3(xInput, 0.0f, yInput);
            weight = new Vector3(1.0f, 0.0f, 1.0f);
        }

        //Mouse movement
        else if(!UIManager.Instance.getMouseOver())
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 screenResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
            int screenEdgeMagnitudeX = (int)((float)screenResolution.x * screenEdgeFraction);
            int screenEdgeMagnitudeY = (int)((float)screenResolution.y * screenEdgeFraction);
            //left
            if (mousePos.x < screenEdgeMagnitudeX)
            {
                direction += Vector3.left;
                weight = new Vector3(
                            Mathf.Clamp(1 - mousePos.x / screenEdgeMagnitudeX, 0.0f, 1.0f),
                            weight.y,
                            weight.z);
            }
            //right
            else if(mousePos.x > screenResolution.x - screenEdgeMagnitudeX)
            {
                direction += Vector3.right;
                weight = new Vector3(
                            Mathf.Clamp(1 - (screenResolution.x - mousePos.x) / screenEdgeMagnitudeX, 0.0f, 1.0f),
                            weight.y,
                            weight.z);
            }
            //down
            if (mousePos.y < screenEdgeMagnitudeY)
            {
                direction += Vector3.back;
                weight = new Vector3(
                            weight.x,
                            weight.y,
                            Mathf.Clamp(1 - mousePos.y / screenEdgeMagnitudeY, 0.0f, 1.0f));
            }
            //forward
            else if (mousePos.y > screenResolution.y - screenEdgeMagnitudeY)
            {
                direction += Vector3.forward;
                weight = new Vector3(
                            weight.x,
                            weight.y,
                            Mathf.Clamp(1 - (screenResolution.y - mousePos.y) / screenEdgeMagnitudeY, 0.0f, 1.0f));
            }
        }
        direction = Vector3.Scale(direction.normalized, weight) * maxCameraSpeed * Time.deltaTime;
        //direction = Vector3.ClampMagnitude(direction.normalized, maxCameraSpeed * weight * Time.deltaTime);

        //Translation and box confine
        transform.Translate(direction);
        transform.position = cameraBoundingBox.ClosestPoint(transform.position);
     
    }
}
