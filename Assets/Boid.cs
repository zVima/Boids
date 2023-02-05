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
  }

  public void SetColor(Color color)
  {

    if (spriteRenderer == null)
    {
      spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
    }

    if (spriteRenderer != null)
    {
      spriteRenderer.color = color;
    }

  }

  public void UpdateBoid(Vector2 velocity) {
    transform.up = velocity;
    transform.position += (Vector3) velocity * settings.speed * Time.deltaTime;

    Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
    if (screenPoint.x < 0) {
      Vector3 newPosition = transform.position;
      newPosition.x = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
      transform.position = newPosition;
    }
    else if (screenPoint.x > Screen.width) {
      Vector3 newPosition = transform.position;
      newPosition.x = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
      transform.position = newPosition;
    }
    if (screenPoint.y < 0) {
      Vector3 newPosition = transform.position;
      newPosition.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
      transform.position = newPosition;
    }
    else if (screenPoint.y > Screen.height) {
      Vector3 newPosition = transform.position;
      newPosition.y = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
      transform.position = newPosition;
    }

  }

}
