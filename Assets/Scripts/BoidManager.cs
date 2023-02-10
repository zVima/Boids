using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
  public BoidSettings settings;
  Boid[] boids;

  private Grid grid;

  // Creating a Grid with a given Cellsize
  void Awake() {
    grid = new Grid(settings.cellSize);
  }

  // Initializing all Boids
  void Start() {
    boids = FindObjectsOfType<Boid> ();
    foreach(Boid boid in boids) {
        boid.Initialize(settings);
    }
  }

  // Calculating the Boids next Move every Frame
  void Update() {
    // Zooming in or out
    Camera camera = GetComponent<Camera>();
    camera.orthographicSize = settings.zoom;

    foreach(Boid boid in boids) {
      List<Transform> nearbyBoids;

      // Getting nearby Boids through either a spatial paritioning Grid or the Collider 2D
      if (settings.gridCalculation) {
        nearbyBoids = grid.GetNearbyBoids(boid, settings.detectionRadius);
      } else {
        nearbyBoids = GetNearbyBoids(boid);
      }

      Vector2 nextMove = Vector2.zero;
      
      // Cohesion
      if(settings.cohesionWeight != 0f) {
        Vector2 cohesion = CalculateCohesion(boid, nearbyBoids);

        if (cohesion.sqrMagnitude > settings.cohesionWeight * settings.cohesionWeight) {
          cohesion.Normalize();
        }
        
        nextMove += cohesion * settings.cohesionWeight;
      }

      // Alignment
      if(settings.alignmentWeight != 0f) {
        Vector2 alignment = CalculateAlignment(boid, nearbyBoids);

        if (alignment.sqrMagnitude > settings.alignmentWeight * settings.alignmentWeight) {
          alignment.Normalize();
        }

        nextMove += alignment * settings.alignmentWeight;
      }

      // Seperation
      if(settings.seperationWeight != 0f) {
        Vector2 seperation = CalculateSeperation(boid, nearbyBoids);

        if (seperation.sqrMagnitude > settings.seperationWeight * settings.seperationWeight) {
          seperation.Normalize();
        }

        nextMove += seperation * settings.seperationWeight;
      }

      if (settings.gridCalculation) {
        grid.RemoveBoid(boid);
      }

      boid.UpdateBoid(nextMove);

      if (settings.gridCalculation) {
        grid.AddBoid(boid);
      }
    }
  }

  // Returns nearby Boids for a given Boid
  List<Transform> GetNearbyBoids(Boid boid) {
    List<Transform> context = new List<Transform>();
    Collider2D[] contextColliders = Physics2D.OverlapCircleAll(boid.transform.position, settings.detectionRadius);
    
    foreach (Collider2D collider in contextColliders) {
      if (collider != boid.BoidCollider) {
        context.Add(collider.transform);
      }
    }
    return context;
  }

  // Calculation for Cohesion Behavior
  Vector2 CalculateCohesion(Boid boid, List<Transform> transformOfNearbyBoids) {
    Vector2 avgPosition = Vector2.zero;
    
    if(transformOfNearbyBoids.Count == 0) {
      return avgPosition;
    }
    
    foreach(Transform transform in transformOfNearbyBoids) {
      avgPosition += (Vector2) transform.position;
    }

    avgPosition /= transformOfNearbyBoids.Count;

    return avgPosition -= (Vector2) boid.transform.position;
  }

  // Calculation for Alignment Behavior
  Vector2 CalculateAlignment(Boid boid, List<Transform> transformOfNearbyBoids) {
    Vector2 avgDirection = Vector2.zero;
   
    if(transformOfNearbyBoids.Count == 0) {
      return avgDirection;
    }

    foreach(Transform transform in transformOfNearbyBoids) {
      avgDirection += (Vector2) transform.up;
    }

    avgDirection /= transformOfNearbyBoids.Count;

    if(avgDirection != Vector2.zero) {
      avgDirection.Normalize();
    }

    return avgDirection;
  }

  // Calculation for Seperation Behavior
  Vector2 CalculateSeperation(Boid boid, List<Transform> transformOfNearbyBoids) {
    Vector2 avgPosition = Vector2.zero;

    if(transformOfNearbyBoids.Count == 0) {
      return avgPosition;
    }

    int boidsInRange = 0;

    foreach(Transform transform in transformOfNearbyBoids) {
      if (Vector2.SqrMagnitude(transform.position - boid.transform.position) < settings.seperationRadius) {
        avgPosition += (Vector2) (boid.transform.position - transform.position);
        boidsInRange++;
      }
    }

    if (boidsInRange > 0) {
      avgPosition /= boidsInRange;
    }

    return avgPosition;
  }
}
