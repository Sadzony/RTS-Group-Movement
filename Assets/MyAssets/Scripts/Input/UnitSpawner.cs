using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    public Button button;
    [SerializeField] private Texture2D addUnitsCursorTexture;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] float holdSpawnRate;
    private Vector2 cursorHotSpot;
    bool hold = false;
    float elapsedTime = 0;
    private void OnEnable()
    {
        button.interactable = false;
        cursorHotSpot = new Vector2(addUnitsCursorTexture.width/2, addUnitsCursorTexture.height/2);
        Cursor.SetCursor(addUnitsCursorTexture, cursorHotSpot, CursorMode.Auto);
    }
    private void OnDisable()
    {
        button.interactable = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !UIManager.Instance.getMouseOver())
        {
            hold = true;
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Find position on navmesh from the hit position
                NavMeshHit navMeshHit;
                if(NavMesh.SamplePosition(hit.point, out navMeshHit, 5.0f, NavMesh.AllAreas))
                {
                    //Spawn unit
                    float yRotation = Random.Range(-179.9f, 179.9f);
                    GameObject.Instantiate(unitPrefab, navMeshHit.position, Quaternion.Euler(0, yRotation, 0));
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        { hold = false; elapsedTime = 0; }
        if(Input.GetMouseButton(0) && hold)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > holdSpawnRate)
            {
                elapsedTime = 0;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    //Find position on navmesh from the hit position
                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(hit.point, out navMeshHit, 5.0f, NavMesh.AllAreas))
                    {
                        //Spawn unit
                        float yRotation = Random.Range(-179.9f, 179.9f);
                        GameObject.Instantiate(unitPrefab, navMeshHit.position, Quaternion.Euler(0, yRotation, 0));
                    }
                }
            }

        }
    }
}
