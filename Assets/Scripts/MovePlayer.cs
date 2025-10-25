using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public float moveSpeed = 10;
    public KeyCode leftKey, rightKey, upKey, downKey;
    public KeyCode driftKey = KeyCode.Space;
    public float maxSpeed = 12f;
    public float acceleration = 30f;
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
    public Vector2 CurrentFacingDir => lastFacingDir;

    private Vector2 input;
    private bool isDrifting;
    private bool isHit;
    // Track key press order for drift orientation resolution
    private float lastDownLeft, lastDownRight, lastDownUp, lastDownDown;

    [Header("Boost")]
    public KeyCode boostKey = KeyCode.LeftShift;
    public float boostMax = 100f;
    public float boostRegenPerSecond = 12f;
    public float boostConsumePerSecond = 40f;
    public float boostSpeedMultiplier = 1.4f;
    public float boostAccelerationMultiplier = 1.4f;
    private float boostCurrent;
    public float BoostFraction => boostMax > 0f ? Mathf.Clamp01(boostCurrent / boostMax) : 0f;
    public bool IsBoosting { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.MoveRotation(0f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boostCurrent = boostMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            float x = (Input.GetKey(rightKey) ? 1f : 0f) - (Input.GetKey(leftKey) ? 1f : 0f);
            float y = (Input.GetKey(upKey) ? 1f : 0f) - (Input.GetKey(downKey) ? 1f : 0f);
            input = new Vector2(x, y);
        }
        else
        {
            input = Vector2.zero;
        }

        // Record press timestamps to know which axis came first
        if (Input.GetKeyDown(leftKey))  lastDownLeft  = Time.time;
        if (Input.GetKeyDown(rightKey)) lastDownRight = Time.time;
        if (Input.GetKeyDown(upKey))    lastDownUp    = Time.time;
        if (Input.GetKeyDown(downKey))  lastDownDown  = Time.time;

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        isDrifting = Input.GetKey(driftKey);

        // Boost logic (resource, regen/consume)
        bool wantsBoost = Input.GetKey(boostKey);
        IsBoosting = wantsBoost && boostCurrent > 0.001f;
        if (IsBoosting)
        {
            boostCurrent -= boostConsumePerSecond * Time.deltaTime;
            if (boostCurrent < 0f) boostCurrent = 0f;
        }
        else
        {
            boostCurrent += boostRegenPerSecond * Time.deltaTime;
            if (boostCurrent > boostMax) boostCurrent = boostMax;
        }

        // Determine display direction (for sprite/rotation) with simple drift visual:
        // Horizontal-first drift (A/D pressed, then W/S + drift): face strictly Up/Down
        // Vertical-first drift (W/S pressed, then A/D + drift): face strictly Left/Right
        bool upPressed = Input.GetKey(upKey);
        bool downPressed = Input.GetKey(downKey);
        bool leftPressed = Input.GetKey(leftKey);
        bool rightPressed = Input.GetKey(rightKey);
        Vector2 displayDir = input;
        bool haveSide = (leftPressed ^ rightPressed);
        bool haveVert = (upPressed ^ downPressed);
        if (isDrifting && haveSide && haveVert)
        {
            float lastHorizDown = Mathf.Max(lastDownLeft, lastDownRight);
            float lastVertDown  = Mathf.Max(lastDownUp, lastDownDown);
            bool horizontalFirst = lastHorizDown < lastVertDown;

            if (horizontalFirst)
            {
                // Face vertical while moving diagonally
                displayDir = upPressed ? Vector2.up : Vector2.down;
            }
            else
            {
                // Face horizontal while moving diagonally
                displayDir = rightPressed ? Vector2.right : Vector2.left;
            }
        }

        if (useDirectionalSprites && spriteRenderer != null)
        {
            ApplyDirectionalSprite(displayDir);
            if (displayDir.sqrMagnitude > 0.0001f)
            {
                lastFacingDir = displayDir;
            }
        }
        else
        {
            if (displayDir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(displayDir.y, displayDir.x) * Mathf.Rad2Deg - 90f;
                rb2d.MoveRotation(angle);
                lastFacingDir = displayDir;
            }
        }
    }

    void FixedUpdate()
    {
        float speedMul = IsBoosting ? boostSpeedMultiplier : 1f;
        float accelMul = IsBoosting ? boostAccelerationMultiplier : 1f;
        Vector2 desiredVelocity = input * (maxSpeed * speedMul);
        float maxDelta = acceleration * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.MoveTowards(rb2d.linearVelocity, desiredVelocity, maxDelta * accelMul);

        float speed = rb2d.linearVelocity.magnitude;
        float cap = maxSpeed * speedMul;
        if (speed > cap)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * cap;
        }
    }

    private void ApplyDirectionalSprite(Vector2 dir)
    {
        if (spriteRenderer == null) return;

        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = (lastFacingDir.sqrMagnitude > 0.0001f) ? lastFacingDir : Vector2.up;
        }

        float x = dir.x;
        float y = dir.y;

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

    public void AddBoost(float amount)
    {
        if (amount <= 0f) return;
        boostCurrent = Mathf.Clamp(boostCurrent + amount, 0f, boostMax);
    }
}


