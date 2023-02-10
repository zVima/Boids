using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawning in Boids at the Start of the Simulation
public class Spawner : MonoBehaviour {
  public Boid prefab;
  public BoidSettings settings;

  List<Boid> boids = new List<Boid>();

  void Awake() {
    // Zooming the Camera by the amount, given in the Settings
    Camera camera = GetComponent<Camera>();
    camera.orthographicSize = settings.zoom;

    // Spawning Boids in a Random Position with Random Directions
    for (int i = 0; i < settings.spawnCount; i++) {
      // Generating a Random Position within the Cameras Boundaries
      float randomX = Random.Range(-Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize * Camera.main.aspect);
      float randomY = Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize);
      Vector2 position = new Vector2(randomX, randomY) + (Vector2)Camera.main.transform.position;

      Boid boid = Instantiate(prefab, position, Quaternion.identity) as Boid;

      // Setting the Startspeed, which is the average of the min and max Speed
      float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
      boid.transform.up = Random.insideUnitCircle * startSpeed;

      boids.Add(boid);
    }
  }
}
