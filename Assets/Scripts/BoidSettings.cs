using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject {
    // Calculation for neighbouring Boids
    public bool gridCalculation = false;
    public float cellSize = 5f;

    // Camera Zoom
    public float zoom = 60f;

    // Amount of Boids
    [Range(10, 5000)]
    public int spawnCount = 250;

    // Settings for the Boid itself
    public float detectionRadius = 0.5f;
    public float seperationRadius = 0.25f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float steerForce = 0.8f;

    // Weights of all Behaviours
    [Range(0f, 1f)]
    public float cohesionWeight = 0.15f;
    [Range(0f, 1f)]
    public float alignmentWeight = 0.15f;
    [Range(0f, 1f)]
    public float seperationWeight = 0.7f;
}
