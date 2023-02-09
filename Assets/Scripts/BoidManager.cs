using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoidStruct {
  public Vector3 position;
  public Vector3 velocity;
  public Color color;
}

public class BoidManager : MonoBehaviour
{

    public BoidSettings settings;
    Boid[] boids;
    Boid selectedBoid;

    public ComputeShader boidShader;

    private int kernelHandle;
    private ComputeBuffer boidsInRangeBuffer;
    private List<BoidStruct> boidsInRange;
  void Start()
  {
    boids = FindObjectsOfType<Boid> ();
    foreach(Boid boid in boids) {
        boid.Initialize(settings);
    }

    kernelHandle = boidShader.FindKernel("CSMain");

    int positionSize = sizeof(float) * 3;
    int velocitySize = sizeof(float) * 3;
    int colorSize = sizeof(float) * 4;
    int stride = positionSize + velocitySize + colorSize;

    boidsInRangeBuffer = new ComputeBuffer(boids.Length, stride);
    boidShader.SetBuffer(kernelHandle, "boidsInRange", boidsInRangeBuffer);

    boidShader.SetFloat("cohesionWeight", settings.cohesionWeight);
    boidShader.SetFloat("alignmentWeight", settings.alignmentWeight);
    boidShader.SetFloat("seperationWeight", settings.seperationWeight);

    boidShader.SetFloat("minSpeed", settings.minSpeed);
    boidShader.SetFloat("maxSpeed", settings.maxSpeed);

    boidShader.SetFloat("seperationRadius", settings.seperationRadius);
    boidShader.SetFloat("steerForce", settings.steerForce);
  }

  void Update()
  {
    // TODO: Neuer Boids In Range Algorithmus (Spatial partioning data structure, grid, octree)
    // TODO: Einbauen des Compute Shaders

    foreach(Boid boid in boids) {
      // List<Transform> nearbyBoidTransforms = GetNearbyBoids(boid);

      // Vector2 nextMove = Vector2.zero;
      
      // // Cohesion
      // if(settings.cohesionWeight != 0f) {
      //   Vector2 cohesion = CalculateCohesion(boid, nearbyBoidTransforms);

      //   if (cohesion.sqrMagnitude > settings.cohesionWeight * settings.cohesionWeight) {
      //     cohesion.Normalize();
      //   }
        
      //   nextMove += cohesion * settings.cohesionWeight;
      // }

      // // Alignment
      // if(settings.alignmentWeight != 0f) {
      //   Vector2 alignment = CalculateAlignment(boid, nearbyBoidTransforms);

      //   if (alignment.sqrMagnitude > settings.alignmentWeight * settings.alignmentWeight) {
      //     alignment.Normalize();
      //   }

      //   nextMove += alignment * settings.alignmentWeight;
      // }

      // // Seperation
      // if(settings.seperationWeight != 0f) {
      //   Vector2 seperation = CalculateSeperation(boid, nearbyBoidTransforms);

      //   if (seperation.sqrMagnitude > settings.seperationWeight * settings.seperationWeight) {
      //     seperation.Normalize();
      //   }

      //   nextMove += seperation * settings.seperationWeight;
      // }

      GetNearbyBoidsStruct(boid);

      boidsInRangeBuffer.SetData(boidsInRange);

      boidShader.Dispatch(kernelHandle, boidsInRange.Count, 1, 1);

      BoidStruct[] boidsInRangeArr = new BoidStruct[boidsInRange.Count];

      boidsInRangeBuffer.GetData(boidsInRangeArr);

      Vector3 nextMove = boidsInRangeArr[0].position * Time.deltaTime;

      boid.UpdateBoid(nextMove);
    }
  }

  void OnDestroy() {
    boidsInRangeBuffer.Dispose();
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

  void GetNearbyBoidsStruct(Boid currentBoid) {
      Collider2D[] contextColliders = Physics2D.OverlapCircleAll(currentBoid.transform.position, settings.detectionRadius);
      int counter = 0;

      boidsInRange = new List<BoidStruct>();

      BoidStruct currentBoidStruct = new BoidStruct();
      currentBoidStruct.position = currentBoid.transform.position;
      currentBoidStruct.velocity = currentBoid.transform.up;
      boidsInRange.Add(currentBoidStruct);

      foreach (Collider2D collider in contextColliders) {
          if (collider != currentBoid.BoidCollider) {
              BoidStruct nearbyBoid = new BoidStruct();
              nearbyBoid.position = collider.transform.position;
              nearbyBoid.velocity = collider.transform.up;
              boidsInRange.Add(nearbyBoid);
              counter++;
          }
      }

      boidShader.SetInt("boidsInRangeCount", counter);
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
