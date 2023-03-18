using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Texture2D addUnitsCursorTexture;
    [SerializeField] GameObject unitPrefab;
    private Vector2 cursorHotSpot;
    private void OnEnable()
    {
        cursorHotSpot = new Vector2(addUnitsCursorTexture.width/2, addUnitsCursorTexture.height/2);
        Cursor.SetCursor(addUnitsCursorTexture, cursorHotSpot, CursorMode.Auto);
    }
    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
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
    }
}
