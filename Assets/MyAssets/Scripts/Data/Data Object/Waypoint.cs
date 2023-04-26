using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    public Waypoint(Vector3 p_position)
    {
        position = p_position;
        originalPosition = p_position;
    }
    public Waypoint(Vector3 p_originalPosition, Vector3 p_offsetPosition)
    {
        originalPosition = p_originalPosition;
        position = p_offsetPosition;
    }
    //Returns true if a position is approximately equal to this waypoints position
    public bool isEqual(Vector3 other)
    {
        Vector3 difference = other - originalPosition;
        if(difference.sqrMagnitude < 1)
        {
            return true;
        }
        return false;
    }
    public Vector3 position;
    public Vector3 originalPosition;
    public Waypoint next;
}
