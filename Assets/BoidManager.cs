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
      // boid.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, nearbyBoidTransforms.Count / 6f);

      Vector2 nextMove = Vector2.zero;
      Vector2 cohesion;
      Vector2 alignment;
      Vector2 seperation;
      int movesAdded = 0;
      if(settings.cohesionActivated) {
        movesAdded++;
        cohesion = CalculateCohesion(boid, nearbyBoidTransforms);

        if (cohesion.sqrMagnitude > settings.cohesionWeight * settings.cohesionWeight) {
          cohesion.Normalize();
          cohesion *= settings.cohesionWeight;
        }

        nextMove += cohesion;
      }
      if(settings.alignmentActivated) {
        movesAdded++;
        alignment = CalculateAlignment(boid, nearbyBoidTransforms);

        if (alignment.sqrMagnitude > settings.alignmentWeight * settings.alignmentWeight) {
          alignment.Normalize();
          alignment *= settings.alignmentWeight;
        }

        nextMove += alignment;
      }
      if(settings.seperationActivated) {
        movesAdded++;
        seperation = CalculateSeperation(boid, nearbyBoidTransforms);

        if (seperation.sqrMagnitude > settings.seperationWeight * settings.seperationWeight) {
          seperation.Normalize();
          seperation *= settings.seperationWeight;
        }

        nextMove += seperation;
      }

      float angle = Vector2.SignedAngle(boid.transform.up, nextMove);
      angle = Mathf.Clamp(angle, -settings.steerForce, settings.steerForce);
      nextMove = Quaternion.Euler(0, 0, angle) * boid.transform.up;

      boid.UpdateBoid(nextMove);
    }
  }

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
