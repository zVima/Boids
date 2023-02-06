using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
  public Boid prefab;
  [Range(10, 2000)]
  public int spawnCount = 250;
  [Range(1f, 10f)]
  public float spawnRadius = 2.5f;
  public float spawnDensity = 0.2f;

  List<Boid> boids = new List<Boid>();

  void Awake()
  {
      for (int i = 0; i < spawnCount; i++)
      {
          Vector2 randomViewport = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
          Vector2 position = Camera.main.ViewportToWorldPoint(randomViewport);
          Boid boid = Instantiate(prefab, position, Quaternion.identity) as Boid;
          boid.transform.up = Random.insideUnitCircle;

          Color randomColor = Color.Lerp(new Color(0f, 75f/255f, 1f), new Color(0f, 140f/255f, 1f), Random.Range(0f, 1f));
          boid.SetColor(randomColor);

          boids.Add(boid);
      }
  }
}
