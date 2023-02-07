using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float detectionRadius = 0.5f;
    public float seperationRadius = 0.25f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float steerForce = 0.8f;

    public bool cohesionActivated = true;
    [Range(0f, 1f)]
    public float cohesionWeight = 0.15f;
    public bool alignmentActivated = true;
    [Range(0f, 1f)]
    public float alignmentWeight = 0.15f;
    public bool seperationActivated = true;
    [Range(0f, 1f)]
    public float seperationWeight = 0.7f;
    public bool avoidanceActivated = false;
    public float wallAvoidanceDist = 0.1f;
}
