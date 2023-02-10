using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{

    public BoidSettings settings;
    Boid[] boids;
    Boid selectedBoid;

    private Grid grid;

  void Awake() {
    grid = new Grid(settings.cellSize);
  }

  void Start()
  {
    boids = FindObjectsOfType<Boid> ();
    foreach(Boid boid in boids) {
        boid.Initialize(settings);
    }

    Camera camera = GetComponent<Camera>();
    camera.orthographicSize = settings.zoom;
  }

  void Update()
  {
    Camera camera = GetComponent<Camera>();
    camera.orthographicSize = settings.zoom;
    
    // TODO: Neuer Boids In Range Algorithmus (Spatial partioning data structure, grid, octree)

    foreach(Boid boid in boids) {
      List<Transform> nearbyBoids;

      if (settings.gridCalculation) {
        nearbyBoids = grid.GetNearbyBoids(boid, settings.detectionRadius);
      }
      nearbyBoids = GetNearbyBoids(boid);

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

      grid.RemoveBoid(boid);

      boid.UpdateBoid(nextMove);

      grid.AddBoid(boid);
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

  // void OnDrawGizmos() {
  //   if (grid == null || settings.cellSize == 0) return;
  //   int gridSize = Mathf.CeilToInt(Camera.main.orthographicSize * 2);

  //   Vector3 bottomLeft = transform.position - Vector3.right * gridSize / 2 - Vector3.up * gridSize / 2;
  //   Vector3 topRight = transform.position + Vector3.right * gridSize / 2 + Vector3.up * gridSize / 2;

  //   int columns = Mathf.CeilToInt(gridSize / settings.cellSize);
  //   int rows = Mathf.CeilToInt(gridSize / settings.cellSize);

  //   for (int i = 0; i <= columns; i++) {
  //     Vector3 start = bottomLeft + Vector3.right * i * settings.cellSize;
  //     Vector3 end = start + Vector3.up * gridSize;
  //     Debug.DrawLine(start, end, Color.grey);
  //   }

  //   for (int i = 0; i <= rows; i++) {
  //     Vector3 start = bottomLeft + Vector3.up * i * settings.cellSize;
  //     Vector3 end = start + Vector3.right * gridSize;
  //     Debug.DrawLine(start, end, Color.grey);
  //   }
  // }

}
