using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public BoidSettings settings;
    Boid[] boids;
    Boid selectedBoid;

  void Start()
  {
    boids = FindObjectsOfType<Boid> ();
    foreach(Boid boid in boids) {
        boid.Initialize(settings);
    }
  }

  void Update()
  {
    foreach(Boid boid in boids) {
      List<Transform> nearbyBoidTransforms = GetNearbyBoids(boid);

      Vector2 nextMove = Vector2.zero;
      
      // Cohesion
      if(settings.cohesionActivated) {
        Vector2 cohesion = CalculateCohesion(boid, nearbyBoidTransforms);

        if (cohesion.sqrMagnitude > settings.cohesionWeight * settings.cohesionWeight) {
          cohesion.Normalize();
          cohesion *= settings.cohesionWeight;
        }

        nextMove += cohesion;
      }

      // Alignment
      if(settings.alignmentActivated) {
        Vector2 alignment = CalculateAlignment(boid, nearbyBoidTransforms);

        if (alignment.sqrMagnitude > settings.alignmentWeight * settings.alignmentWeight) {
          alignment.Normalize();
          alignment *= settings.alignmentWeight;
        }

        nextMove += alignment;
      }

      // Seperation
      if(settings.seperationActivated) {
        Vector2 seperation = CalculateSeperation(boid, nearbyBoidTransforms);

        if (seperation.sqrMagnitude > settings.seperationWeight * settings.seperationWeight) {
          seperation.Normalize();
          seperation *= settings.seperationWeight;
        }

        nextMove += seperation;
      }

      boid.UpdateBoid(nextMove);
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

  // Calculation for Avoidance Behavior (EXPERIMENTAL)
  Vector2 CalculateAvoidance(Boid boid) {
    Vector3 newPos = boid.transform.up;
    Vector3 screenPoint = Camera.main.WorldToScreenPoint(newPos);

    if(screenPoint.x < 0 + settings.wallAvoidanceDist) {
      newPos.x = -newPos.x;
    } else if (screenPoint.x > Screen.width + settings.wallAvoidanceDist) {
      newPos.x = -newPos.x;
    }

    if (screenPoint.y < 0 + settings.wallAvoidanceDist) {
      newPos.y = -newPos.y;
    } else if (screenPoint.y > Screen.height + settings.wallAvoidanceDist) {
      newPos.y = -newPos.y;
    }

    return newPos;
  }

}
