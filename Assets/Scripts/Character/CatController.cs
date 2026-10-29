using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CatController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 15f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.2f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float coyoteTime = 0.12f;    // can still jump just after leaving a ledge
    [SerializeField] private float jumpBuffer = 0.12f;    // early jump press is remembered
    [SerializeField] private float jumpCut = 0.5f;        // releasing jump early cuts the jump short

    [Header("Fall")]
    [SerializeField] private float fallY = -10f;          // below this height = fail

    [Header("References")]
    [SerializeField] private Animator animator;           // the cat Model's Animator

    [Header("Squash and Stretch")]
    [SerializeField] private float squashRecovery = 12f;

    private CharacterController cc;
    private Vector3 startPos;
    private Vector3 horizontal;       // current sideways velocity
    private float verticalVelocity;
    private float coyoteTimer;
    private float bufferTimer;
    private Vector3 baseScale;     // the Model's normal scale
    private bool wasGrounded;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        startPos = transform.position;

        baseScale = animator.transform.localScale;
    }

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
        GameManager.LevelReset += ResetCat;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
        GameManager.LevelReset -= ResetCat;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        bool playing = GameManager.Instance == null || GameManager.Instance.State == GameState.Playing;

        // 1. Move relative to the camera
        Vector3 wish = Vector3.zero;
        if (playing)
        {
            Transform cam = Camera.main.transform;
            Vector3 fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            wish = Vector3.ClampMagnitude(fwd * v + right * h, 1f) * moveSpeed;
        }
        horizontal = Vector3.MoveTowards(horizontal, wish, acceleration * dt);

        // 2. Ground and timers
        bool grounded = cc.isGrounded;
        if (grounded)
        {
            if (!wasGrounded && verticalVelocity < -5f) Land();
            coyoteTimer = coyoteTime;
            if (verticalVelocity < 0f) verticalVelocity = -2f;   // small push keeps us stuck to the ground
        }
        else
        {
            coyoteTimer -= dt;
        }
        wasGrounded = grounded;

        bufferTimer -= dt;
        if (playing && Input.GetButtonDown("Jump")) bufferTimer = jumpBuffer;

        // 3. Jump
        if (bufferTimer > 0f && coyoteTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            bufferTimer = 0f;
            coyoteTimer = 0f;

            Kick(0.8f, 1.25f);   
        }

        // Releasing the button while rising makes a shorter jump
        if (Input.GetButtonUp("Jump") && verticalVelocity > 0f)
            verticalVelocity *= jumpCut;

        animator.transform.localScale =
            Vector3.Lerp(animator.transform.localScale, baseScale, squashRecovery * dt);

        // 4. Gravity
        verticalVelocity = Mathf.Max(verticalVelocity + gravity * dt, -30f);

        // 5. Apply movement
        cc.Move((horizontal + Vector3.up * verticalVelocity) * dt);

        // 6. Face the move direction
        if (horizontal.sqrMagnitude > 0.1f)
        {
            Quaternion target = Quaternion.LookRotation(horizontal);
            animator.transform.rotation =
                Quaternion.Slerp(animator.transform.rotation, target, turnSpeed * dt);
        }

        // 7. Animator
        animator.SetFloat("Speed", horizontal.magnitude / moveSpeed, 0.1f, dt);
        animator.SetBool("Grounded", grounded);

        // 8. Fell off the level
        if (playing && transform.position.y < fallY)
            GameManager.Instance.Fail();
    }

    // Temporarily deforms the Model; the Lerp above eases it back
    private void Kick(float xz, float y)
    {
        animator.transform.localScale = Vector3.Scale(baseScale, new Vector3(xz, y, xz));
        if (SoundManager.Instance != null) SoundManager.Instance.PlayJump();
    }

    private void Land()
    {
        animator.transform.localScale = Vector3.Scale(baseScale, new Vector3(1.25f, 0.75f, 1.25f));
        if (SoundManager.Instance != null) SoundManager.Instance.PlayLand();
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Won) animator.SetTrigger("Win");
        else if (state == GameState.Failed) animator.SetTrigger("Fail");
    }

    private void ResetCat()
    {
        cc.enabled = false;               // a CharacterController must be off to teleport it
        transform.position = startPos;
        cc.enabled = true;

        horizontal = Vector3.zero;
        verticalVelocity = 0f;
        coyoteTimer = 0f;
        bufferTimer = 0f;

        animator.transform.localRotation = Quaternion.identity;
        animator.Rebind();                // back to the default Animator state
        animator.Update(0f);

        animator.transform.localScale = baseScale;
    }
}