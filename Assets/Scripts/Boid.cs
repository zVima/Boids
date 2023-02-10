using UnityEngine;

public class Boid : MonoBehaviour {
  // Collider to calculate nearby Boids
  Collider2D boidCollider;
  public Collider2D BoidCollider { get { return boidCollider; } }

  BoidSettings settings;

  // Cached
  Transform cachedTransform;

  // Caching the Transfrom for performance
  void Awake() {
    cachedTransform = transform;
  }

  // Initializing the boid
  public void Initialize(BoidSettings settings)
  {
    this.settings = settings;
    boidCollider = GetComponent<Collider2D>();
  }

  // Updating the boids Position every Frame
  public void UpdateBoid(Vector2 velocity) {
    Vector3 newPos;
    float smoothSpeed = Sigmoid(velocity.magnitude);
    float speed = Mathf.Lerp(settings.minSpeed, settings.maxSpeed, smoothSpeed);
    
    if (velocity == Vector2.zero) {
      newPos = cachedTransform.up;
    } else {
      newPos = (Vector3) velocity;
    }

    cachedTransform.up = SteerTowards(newPos);

    cachedTransform.position += cachedTransform.up * Time.deltaTime * speed;

    wrapAround();
  }

  // Standard Sigmoid calculation
  private float Sigmoid(float x) {
    return 1 / (1 + Mathf.Exp(-x));
  }

  // Smoothly steering towards the target Vector
  public Vector3 SteerTowards(Vector3 targetVector) {
    float angle = Vector2.SignedAngle(cachedTransform.up, targetVector);
    angle = Mathf.Clamp(angle, -settings.steerForce, settings.steerForce);
    return Quaternion.Euler(0, 0, angle) * cachedTransform.up;
  }

  // Wrapping the Boid around to the other side, when it leaves one
  void wrapAround() {
    Vector3 newPosition = cachedTransform.position;
    Vector3 screenPoint = Camera.main.WorldToScreenPoint(newPosition);
    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    if (!screenRect.Contains(screenPoint)) {
      Vector3 screenBounds = screenRect.size;
      screenBounds.z = 0;
      Vector3 worldBounds = Camera.main.ScreenToWorldPoint(screenBounds);

      if (screenPoint.x < 0) {
        newPosition.x = worldBounds.x;
      } else if (screenPoint.x > Screen.width) {
        newPosition.x = -worldBounds.x;
      }

      if (screenPoint.y < 0) {
        newPosition.y = worldBounds.y;
      } else if (screenPoint.y > Screen.height) {
        newPosition.y = -worldBounds.y;
      }
    }
    cachedTransform.position = newPosition;
  }
}
