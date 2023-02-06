using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boid : MonoBehaviour
{

  Collider2D boidCollider;
  public Collider2D BoidCollider { get { return boidCollider; } }

  BoidSettings settings;
  SpriteRenderer spriteRenderer;

  public void Initialize(BoidSettings settings)
  {
    this.settings = settings;
    boidCollider = GetComponent<Collider2D>();

    float startSpeed = (settings.minSpeed * settings.maxSpeed) / 2;
    // transform.position = transform.position * startSpeed;
  }

  public void SetColor(Color color)
  {

    if (spriteRenderer == null) {
      spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
    }

    if (spriteRenderer != null) {
      spriteRenderer.color = color;
    }

  }

  public void UpdateBoid(Vector2 velocity) {
    Vector3 newPosition = transform.position;
    Vector3 screenPoint = Camera.main.WorldToScreenPoint(newPosition);
    if (screenPoint.x < 0) {
      newPosition.x = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
    }
    else if (screenPoint.x > Screen.width) {
      newPosition.x = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
    }
    
    if (screenPoint.y < 0) {
      newPosition.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
    }
    else if (screenPoint.y > Screen.height) {
      newPosition.y = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
    }

    transform.position = newPosition;
    transform.up = velocity;
    transform.position += (Vector3) velocity * settings.speed * Time.deltaTime;

  }

}
