using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    public Waypoint(Vector3 p_position)
    {
        position = p_position;
    }
    //Returns true if a position is approximately equal to this waypoints position
    public bool isEqual(Vector3 other)
    {
        Vector3 difference = other - position;
        if(difference.sqrMagnitude < 1)
        {
            return true;
        }
        return false;
    }
    public Vector3 position;
    public Waypoint next;
    public HashSet<Waypoint> branches = new HashSet<Waypoint>();
}
