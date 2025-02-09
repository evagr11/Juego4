
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Movement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    [SerializeField] private MovementStats stats;
    public PhysicsMaterial2D noFriction;

    [Header("Ground Check")]
    private Vector2 direction;
    [SerializeField] private LayerMask groundLayer;
    private bool isOnGround = false;
    private bool wasOnGroundPreviousFrame = false;
    private float groundCheckDistance = 0.1f;
    [SerializeField] private Transform groundCheckOrigin;

    [Header("Jump Execution")]
    private float pressTime = 0f;
    private bool isJumping = false;
    public float jumpCount;

    [Header("Particles")]
    public SpriteRenderer spriteRenderer;
    public ParticleSystem jumpParticles;
    public ParticleSystem noJumpParticles;
    public ParticleSystem collisionParticles;
    public ParticleSystem movementParticles;
    public ParticleSystem.MainModule mainModule;

    [Header("Colors")]
    [SerializeField] private Gradient onAirColors;
    [SerializeField] private Color groundedColor;
    [SerializeField] private Color risingColor;
    [SerializeField] private Color peakColor;
    [SerializeField] private Color fallingColor;
    [SerializeField] private Color groundColor;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        noFriction = GetComponent<PhysicsMaterial2D>();
        mainModule = movementParticles.main;
        jumpParticles.Stop();
        noJumpParticles.Stop();
        collisionParticles.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
        }

        isOnGround = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckDistance, groundLayer);

        if (!wasOnGroundPreviousFrame && isOnGround)
        {
            collisionParticles.Play();
        }

        if (isOnGround)
        {
            spriteRenderer.color = groundedColor;
            mainModule.startColor = groundColor;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isOnGround || jumpCount <= stats.onAirJumps)
            {
                if (isOnGround)
                    jumpCount = 0;
                else
                    jumpCount++;

                spriteRenderer.color = onAirColors.Evaluate(jumpCount / stats.onAirJumps);

                jumpParticles.Play();
            }
            else
            {
                noJumpParticles.Play();
            }
        }

        if (Input.GetKey(KeyCode.W) && pressTime < stats.maxJumpTime && jumpCount <= stats.onAirJumps)
        {
            pressTime += Time.deltaTime;
            isJumping = true; 
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            pressTime = 0;
        }

        if (rb.velocity.y > stats.yVelocityLowGravityThreshold)
        {
            mainModule.startColor = risingColor;
            rb.gravityScale = stats.risingtGravity;
        }
        else if (rb.velocity.y < -stats.yVelocityLowGravityThreshold)
        {
            mainModule.startColor = fallingColor;
            rb.gravityScale = stats.fallingGravity;
        }
        else if (stats.yVelocityLowGravityThreshold > rb.velocity.y && rb.velocity.y > -stats.yVelocityLowGravityThreshold && !isOnGround)
        {
            mainModule.startColor = peakColor;
            rb.gravityScale = stats.peakGravity;
        }

        wasOnGroundPreviousFrame = isOnGround;
    }

    void FixedUpdate()
    {
        if (isOnGround)
        {
            // Si la velocidad horizontal está dentro del límite, aumenta la velocidad con la aceleración del suelo
            if (rb.velocity.x < stats.maxGroundHorizontalSpeed && rb.velocity.x > -stats.maxGroundHorizontalSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x + (direction.x * stats.groundAcceleration * Time.deltaTime), rb.velocity.y);
            }
            else
            {
                // Si supera la velocidad máxima, se restringe al máximo permitido
                if (rb.velocity.x > stats.maxGroundHorizontalSpeed)
                    rb.velocity = new Vector2(stats.maxGroundHorizontalSpeed, rb.velocity.y);
                else if (rb.velocity.x < -stats.maxGroundHorizontalSpeed)
                    rb.velocity = new Vector2(-stats.maxGroundHorizontalSpeed, rb.velocity.y);
            }
        }
        else
        {
            // Si la velocidad horizontal está dentro del límite, aplica la aceleración para moverse en el aire
            if (rb.velocity.x < stats.maxAirHorizontalSpeed && rb.velocity.x > -stats.maxAirHorizontalSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x + (direction.x * stats.airAcceleration * Time.deltaTime), rb.velocity.y);
            }
            else
            {
                // Si la velocidad supera el máximo, se restringe
                if (rb.velocity.x > stats.maxAirHorizontalSpeed)
                    rb.velocity = new Vector2(stats.maxAirHorizontalSpeed, rb.velocity.y);
                else if (rb.velocity.x < -stats.maxAirHorizontalSpeed)
                    rb.velocity = new Vector2(-stats.maxAirHorizontalSpeed, rb.velocity.y);
            }
        }

        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, stats.jumpStrength);
            isJumping = false;
        }

        if (isOnGround)
        {
            // Si el personaje se mueve a la derecha, reduce la velocidad aplicando la fricción del suelo
            if (rb.velocity.x > 0)
                rb.velocity = new Vector2(rb.velocity.x - stats.groundHorizontalFriction * Time.deltaTime, rb.velocity.y);
            // Idem para la izquierda
            else if (rb.velocity.x < 0)
                rb.velocity = new Vector2(rb.velocity.x + stats.groundHorizontalFriction * Time.deltaTime, rb.velocity.y);
        }
        else //mismo mecanismo para cuando esta en el aire
        {
            if (rb.velocity.x > 0)
                rb.velocity = new Vector2(rb.velocity.x - stats.airFriccion * Time.deltaTime, rb.velocity.y);
            else if (rb.velocity.x < 0)
                rb.velocity = new Vector2(rb.velocity.x + stats.airFriccion * Time.deltaTime, rb.velocity.y);
        }

        if (rb.velocity.y < -stats.maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -stats.maxFallSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckOrigin.position, 0.01f);
        Gizmos.DrawLine(groundCheckOrigin.position, groundCheckOrigin.position + Vector3.down * groundCheckDistance);
    }

    public void UpdateStats(MovementStats newStats)
    {
        stats = newStats;
    }
}
