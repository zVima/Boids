using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
  public Boid prefab;
  public BoidSettings settings;

  List<Boid> boids = new List<Boid>();

  void Awake()
  {
      for (int i = 0; i < settings.spawnCount; i++)
      {
          Vector2 randomViewport = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
          Vector2 position = Camera.main.ViewportToWorldPoint(randomViewport);

          Boid boid = Instantiate(prefab, position, Quaternion.identity) as Boid;

          float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
          boid.transform.up = Random.insideUnitCircle * startSpeed;

          boids.Add(boid);
      }
  }
}
