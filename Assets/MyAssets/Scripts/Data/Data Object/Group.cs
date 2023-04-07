using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    private Squad owner;
    private HashSet<Unit> members = new HashSet<Unit>();

    public void JoinGroup(Unit unit)
    {
        members.Add(unit);

        //Add group manager dictionary entry, at owner squad
    }
    public void LeaveGroup(Unit unit)
    {
        members.Remove(unit);
        //Remove group manager dictionary entry, at owner squad
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
