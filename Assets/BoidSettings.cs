using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float detectionRadius = 0.5f;
    public float seperationRadius = 0.25f;
    public float speed = 0.8f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float steerForce = 0.8f;

    public bool cohesionActivated = true;
    public float cohesionWeight = 0.15f;
    public bool alignmentActivated = true;
    public float alignmentWeight = 0.15f;
    public bool seperationActivated = true;
    public float seperationWeight = 0.7f;
}
