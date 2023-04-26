using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringData : MonoBehaviour
{
    //Singleton Template
    private static SteeringData instance;
    public static SteeringData Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    [Range(1.0f, 10.0f)] public float unitSpeed;
    [Range(0.01f, 1.0f)]public float unitRotationTime;
    [Space(20)]


    public bool neighbourLoSArcEnabled;
    [Range(10.0f, 180.0f)] public float neighbourLoSArc;
    [Space(20)]

    [Range(0.1f, 5.0f)] public float waypointNearDistance;
    [HideInInspector, Min(1.0f)] public float waypointNearDistanceSqr;
    [Range(1.0f, 15.0f)] public float goalAreaGrowthMultiplier;
    [Range(0.1f, 10.0f)] public float goalReachedSpeed;
    [Space(10)]
    public bool arriveEnabled;
    [Space(10)]
    public bool finishMessagingEnabled;
    [Space(20)]


    [Range(0.0f, 1.0f)] public float steeringPower;
    [Range(0.0f, 1.0f)] public float flockingPower;
    [Space(20)]

    [Range(0.0f, 1.0f)] public float seekWeight;
    [Space(10)]

    [Range(0.0f, 1.0f)] public float separationWeight;
    [Range(1.0f, 15.0f)] public float separationDistance;
    [HideInInspector, Min(1.0f)] public float separationDistanceSqr;
    [Space(20)]


    [Range(0.0f, 1.0f)] public float cohesionWeight;
    [Range(1.0f, 15.0f)] public float cohesionDistance;
    [HideInInspector, Min(1.0f)] public float cohesionDistanceSqr;
    [Space(10)]



    [Range(0.0f, 1.0f)] public float alignmentWeight;
    [Range(1.0f, 15.0f)] public float alignmentDistance;
    [HideInInspector, Min(1.0f)] public float alignmentDistanceSqr;

    private void OnValidate()
    {
        waypointNearDistanceSqr = waypointNearDistance * waypointNearDistance;
        cohesionDistanceSqr = cohesionDistance * cohesionDistance;
        separationDistanceSqr = separationDistance * separationDistance;
        alignmentDistanceSqr = alignmentDistance * alignmentDistance;
    }

}
