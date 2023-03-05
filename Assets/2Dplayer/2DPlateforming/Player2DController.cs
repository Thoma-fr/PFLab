using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PFenum
{
    None = 0,
    Basic =1,
    Bounce=2,
    Boost=3,
    Lazer=4,
    Mirror=5,
    Gravity=6,

}
public class Player2DController : MonoBehaviour
{
    public static Player2DController instance;
    public PFenum choosenPF;
    public GameObject[] pfPrefabs;
    public GameObject mouseFollow;
    public float rotationMultiplyer;
    [SerializeField] private GameObject pfContainer;
    public float life;
    public float damage;
    private bool canRotate;
    [Header("Stats")]
    [SerializeField] private float maxlife;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float coins;
    [SerializeField] private GameObject playerVisual;
    private Rigidbody2D rb;
    private float directionX;
    private Animator animator;
    private SpriteRenderer sprite;

    [Header("Jump")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityMultiplyer;
    [SerializeField] private float fallGravity;
    [SerializeField] float coyoteTime;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferCounter;

    private CapsuleCollider2D col;
    private Coroutine _jumpCoroutine;
    private float duration;
    private float timer;

    private Vector2 direction;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    [SerializeField] private float frictionAnmount;
    [SerializeField] private float groundedSize;
    Vector3 mousePosition;
    GameObject newPF;
    private Vector2 velocity;
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        sprite = playerVisual.GetComponent<SpriteRenderer>();
        animator = playerVisual.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        baseSpeed=speed;
        pfContainer.SetActive(false);
        
    }
    public void SelectPF(int id)
    {
        choosenPF = (PFenum)id;
        Debug.Log("PFselect");
    }
    public void spawnPF(InputAction.CallbackContext context)
    {
        if(context.started)
            newPF = Instantiate(pfPrefabs[(int)choosenPF], mouseFollow.transform.position, Quaternion.identity);
        if (context.performed)
        {
            if(Mouse.current.leftButton.IsPressed())
            {
                canRotate = true;
                Debug.Log("test");
            }

        }
        if(context.canceled)
        {
            canRotate = false;
        }

    }
    private void Update()
    {
        FollowMouse();

        if (GroundCheck())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter-= Time.deltaTime;
        
        if(canRotate)
        {
            
            //newPF.transform.LookAt(mousePosition);
            Vector3 dir = newPF.transform.InverseTransformPoint(mousePosition);
            var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg-90;
            newPF.transform.Rotate(0, 0, angle);

            //float dist = Vector3.Distance(newPF.transform.position, mousePosition);
            //Debug.Log(dist);
            //if(newPF.transform.position.x> mousePosition.x)
            //    newPF.transform.rotation = Quaternion.Euler(0, 0, dist* rotationMultiplyer);
            //else
            //    newPF.transform.rotation = Quaternion.Euler(0, 0, dist * -rotationMultiplyer);
        }
    }
    public void FollowMouse()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        mouseFollow.transform.position = mousePosition;

    }
    private void FixedUpdate()
    {

        float targetSpeed = direction.x * speed;
        if (!GroundCheck())
            targetSpeed *= jumpSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.001f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
        if (direction.x == 0 && rb.velocity.x != 0)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAnmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        velocity = context.ReadValue<Vector2>();
        direction = context.ReadValue<Vector2>();
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VelocityY", rb.velocity.y);
        if (rb.velocity.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;


    }
    public void OpenChoice (InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pfContainer.SetActive(true);
            Time.timeScale = 0.1f;
            pfContainer.GetComponent<PFUIContainer>().AnimeUI();
        }
        else
        {
            pfContainer.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void TakeDamage(float damage)
    {
        life -= damage;
    }
    public void GiveHeal(float Heal)
    {
        life += Heal;
    }
    public void AddCoin(float value)
    {
        coins += value;
    }
    public bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, col.bounds.min.y), -gameObject.transform.up, groundedSize);
        Debug.DrawRay(new Vector2(transform.position.x, col.bounds.min.y), -gameObject.transform.up * groundedSize, Color.magenta);

        return hit;
    }
    public bool HeadCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, col.bounds.max.y), gameObject.transform.up, groundedSize);
        Debug.DrawRay(new Vector2(transform.position.x, col.bounds.max.y), gameObject.transform.up * groundedSize, Color.magenta);

        return hit;
    }
    public void jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //jumpBufferCounter = jumpBufferTime;
            if (coyoteTimeCounter > 0f)
                _jumpCoroutine = StartCoroutine(JumpCoroutine());
        }
        else if (context.canceled)
        {
            if (_jumpCoroutine != null)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
                coyoteTimeCounter = 0f;
            }

            rb.gravityScale = fallGravity;
        }
    }
    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        float t = 0;
        rb.gravityScale = 0;
        while (t < 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.LerpUnclamped(jumpForce, 0, jumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / jumpDuration;
            if (HeadCheck())
                break;
            yield return new WaitForFixedUpdate();
        }
        isJumping=false;
        rb.gravityScale = fallGravity;
        _jumpCoroutine = null;
        coyoteTimeCounter = 0f;
        yield break;
    }

}
