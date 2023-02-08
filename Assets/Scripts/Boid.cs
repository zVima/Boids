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

    if (spriteRenderer == null) {
      spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
    }

    if (spriteRenderer != null) {
      spriteRenderer.color = color;
    }

  }

  public void UpdateBoid(Vector2 velocity) {
    Vector3 newPos;
    float smoothSpeed = Sigmoid(velocity.magnitude);
    float speed = Mathf.Lerp(settings.minSpeed, settings.maxSpeed, smoothSpeed);
    
    if (velocity == Vector2.zero) {
      newPos = transform.up;
    } else {
      newPos = (Vector3) velocity;
    }

    transform.up = SteerTowards(newPos);

    transform.position += transform.up * Time.deltaTime * speed;

    wrapAround();
  }

  private float Sigmoid(float x) {
    return 1 / (1 + Mathf.Exp(-x));
  }

  public Vector3 SteerTowards(Vector3 vector) {
    float angle = Vector2.SignedAngle(transform.up, vector);
    angle = Mathf.Clamp(angle, -settings.steerForce, settings.steerForce);
    return Quaternion.Euler(0, 0, angle) * transform.up;
  }

  void wrapAround() {
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
  }

}
