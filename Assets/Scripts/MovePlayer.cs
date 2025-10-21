using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public float moveSpeed = 10;
    public KeyCode leftKey, rightKey, upKey, downKey;
    public KeyCode driftKey = KeyCode.Space;

    public float maxSpeed = 12f;
    public float acceleration = 30f;
    public float driftFactorSticky = 0.15f; // High grip (no drift)
    public float driftFactorSlippy = 0.98f; // Low grip (drifting)

    [Header("Directional Sprite Mode (8-way)")]
    public bool useDirectionalSprites = false;
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    public Sprite spriteUpLeft;
    public Sprite spriteUpRight;
    public Sprite spriteDownLeft;
    public Sprite spriteDownRight;
    private SpriteRenderer spriteRenderer;

    private Vector2 lastFacingDir = Vector2.up;

    private Vector2 input;
    private bool isDrifting;

    // External stun control (e.g., from crash)
    private bool externallyStunned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Grab the Rigidbody component
        rb2d = GetComponent<Rigidbody2D>();
        // Ensure the car starts facing up (W)
        rb2d.MoveRotation(0f);

        // Try get sprite renderer for directional sprites
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // When stunned, ignore fresh inputs but let physics carry motion
        if (!externallyStunned)
        {
            // Read input from assigned keys and build a direction vector
            float x = (Input.GetKey(rightKey) ? 1f : 0f) - (Input.GetKey(leftKey) ? 1f : 0f);
            float y = (Input.GetKey(upKey) ? 1f : 0f) - (Input.GetKey(downKey) ? 1f : 0f);
            input = new Vector2(x, y);
        }
        else
        {
            input = Vector2.zero;
        }

        // Normalize to keep diagonal speed consistent
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        // Drift toggle
        isDrifting = Input.GetKey(driftKey);

        if (useDirectionalSprites && spriteRenderer != null)
        {
            ApplyDirectionalSprite(input);
            if (input.sqrMagnitude > 0.0001f)
            {
                lastFacingDir = input;
            }
        }
        else
        {
            // Face the movement direction (including diagonals). Default up (W) is angle 0.
            if (input.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg - 90f;
                rb2d.MoveRotation(angle);
                lastFacingDir = input;
            }
        }
    }

    void FixedUpdate()
    {
        // Acceleration-based movement toward desired velocity
        Vector2 desiredVelocity = input * maxSpeed;
        float maxDelta = acceleration * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.MoveTowards(rb2d.linearVelocity, desiredVelocity, maxDelta);

        // Determine forward/right basis for drift. If using sprites (no rotation), derive from velocity/lastFacingDir
        Vector2 forwardDir;
        if (useDirectionalSprites)
        {
            if (rb2d.linearVelocity.sqrMagnitude > 0.0001f)
            {
                forwardDir = rb2d.linearVelocity.normalized;
            }
            else
            {
                forwardDir = (lastFacingDir.sqrMagnitude > 0.0001f) ? lastFacingDir : Vector2.up;
            }
        }
        else
        {
            forwardDir = transform.up;
        }

        Vector2 rightDir = new Vector2(forwardDir.y, -forwardDir.x);
        Vector2 forwardVelocity = forwardDir * Vector2.Dot(rb2d.linearVelocity, forwardDir);
        Vector2 lateralVelocity = rightDir * Vector2.Dot(rb2d.linearVelocity, rightDir);

        float driftFactor = isDrifting ? driftFactorSlippy : driftFactorSticky;
        rb2d.linearVelocity = forwardVelocity + lateralVelocity * driftFactor;

        // Cap max speed
        float speed = rb2d.linearVelocity.magnitude;
        if (speed > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    private void ApplyDirectionalSprite(Vector2 dir)
    {
        if (spriteRenderer == null) return;

        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = (lastFacingDir.sqrMagnitude > 0.0001f) ? lastFacingDir : Vector2.up;
        }

        // Use 8-way selection based on input direction
        float x = dir.x;
        float y = dir.y;

        // Small deadzone to stabilize selection
        const float dead = 0.0001f;
        if (Mathf.Abs(x) < dead) x = 0f;
        if (Mathf.Abs(y) < dead) y = 0f;

        if (x > 0f && y > 0f)
        {
            spriteRenderer.sprite = spriteUpRight != null ? spriteUpRight : (spriteRight != null ? spriteRight : spriteUp);
        }
        else if (x < 0f && y > 0f)
        {
            spriteRenderer.sprite = spriteUpLeft != null ? spriteUpLeft : (spriteLeft != null ? spriteLeft : spriteUp);
        }
        else if (x > 0f && y < 0f)
        {
            spriteRenderer.sprite = spriteDownRight != null ? spriteDownRight : (spriteRight != null ? spriteRight : spriteDown);
        }
        else if (x < 0f && y < 0f)
        {
            spriteRenderer.sprite = spriteDownLeft != null ? spriteDownLeft : (spriteLeft != null ? spriteLeft : spriteDown);
        }
        else if (x > 0f)
        {
            if (spriteRight != null) spriteRenderer.sprite = spriteRight;
        }
        else if (x < 0f)
        {
            if (spriteLeft != null) spriteRenderer.sprite = spriteLeft;
        }
        else if (y > 0f)
        {
            if (spriteUp != null) spriteRenderer.sprite = spriteUp;
        }
        else if (y < 0f)
        {
            if (spriteDown != null) spriteRenderer.sprite = spriteDown;
        }
    }

    public void SetExternalStun(bool isStunned)
    {
        externallyStunned = isStunned;
    }
}


