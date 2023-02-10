using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float zoom = 60f;
    public bool gridCalculation = true;
    public float cellSize = 5f;

    [Range(10, 5000)]
    public int spawnCount = 250;

    public float detectionRadius = 0.5f;
    public float seperationRadius = 0.25f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float steerForce = 0.8f;

    [Range(0f, 1f)]
    public float cohesionWeight = 0.15f;
    [Range(0f, 1f)]
    public float alignmentWeight = 0.15f;
    [Range(0f, 1f)]
    public float seperationWeight = 0.7f;
}
