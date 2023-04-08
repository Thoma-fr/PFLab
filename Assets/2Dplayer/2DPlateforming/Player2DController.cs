using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.U2D;

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
    public GameObject choosenPlatformGameobject;
    public GameObject[] pfPrefabs;
    //public GameObject mouseFollow;
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
    public float timeScaleValue;
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
    [SerializeField] private Vector2 velocity;
    [SerializeField] private float _accelerationStrength;
    [SerializeField] private float _airAccelerationFactor;
    [SerializeField] private bool isMoving;
    [SerializeField] private float _deccelerationStrength;

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
        choosenPF = PFenum.None;
    }
    public void SelectPF(int id)
    {
        choosenPF = (PFenum)id;

        if(choosenPlatformGameobject != null)
            Destroy(choosenPlatformGameobject);

        choosenPlatformGameobject = Instantiate(pfPrefabs[(int)choosenPF], mousePosition, Quaternion.identity);
        choosenPlatformGameobject.GetComponent<Platform>().Ghostify();
        //Vector4 color = pfPrefabs[(int)choosenPF].GetComponentInChildren<SpriteRenderer>().color;
        //mouseFollow.GetComponent<SpriteRenderer>().color = new Vector4(color.x, color.y, color.z, mouseFollow.GetComponent<SpriteRenderer>().color.a);
        //Debug.Log("PFselect");
    }
    public void spawnPF(InputAction.CallbackContext context)
    {
        if (choosenPF == PFenum.None) return;

        if (context.started)
        {
            newPF = Instantiate(pfPrefabs[(int)choosenPF], mousePosition, Quaternion.identity);
            newPF.GetComponent<Platform>().Ghostify();
            choosenPlatformGameobject.SetActive(false);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScaleValue, 0.2f);
            //Time.timeScale = timeScaleValue*3;
        }
        if (context.performed)
        {
            canRotate = true;
            //if(Mouse.current.leftButton.IsPressed())
            //{
            //    canRotate = true;
            //    Debug.Log("test");
            //}
        }
        if(context.canceled)
        {
            canRotate = false;
            newPF.GetComponent<Platform>().RenderPhysical();
            choosenPlatformGameobject.SetActive(true);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f);
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
            Vector3 dir = newPF.transform.InverseTransformPoint(mousePosition);
            var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg-90;
            newPF.transform.Rotate(0, 0, angle);

        }
    }

    public void FollowMouse()
    {
        if (choosenPF == PFenum.None) return;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        if(choosenPlatformGameobject)
            choosenPlatformGameobject.transform.position = mousePosition;
    }
    private void FixedUpdate()
    {
        if (!isMoving)
            Decceleration();


        if (GroundCheck())
            CapSpeed();
        //float targetSpeed = direction.x * speed;
        //if (!GroundCheck())
        //    targetSpeed *= jumpSpeed;
        //float speedDif = targetSpeed - rb.velocity.x;
        //float accelRate = (Mathf.Abs(targetSpeed) > 0.001f) ? acceleration : decceleration;
        //float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        //rb.AddForce(movement * Vector2.right);
        //if (direction.x == 0 && rb.velocity.x != 0)
        //{
        //    float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAnmount));
        //    amount *= Mathf.Sign(rb.velocity.x);
        //    rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        //}

    }
    private void Decceleration()
    {
        rb.velocity -= _deccelerationStrength * rb.velocity.normalized;

        if (rb.velocity.sqrMagnitude < 0.5f)
        {
            rb.velocity = Vector2.zero;
        }
    }
    private void CapSpeed()
    {
        if (rb.velocity.sqrMagnitude > speed * speed)
            rb.velocity = speed * rb.velocity.normalized;
    }
    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            velocity = context.ReadValue<Vector2>();
            direction = context.ReadValue<Vector2>();
            isMoving = true;
            if (GroundCheck())
                rb.velocity += _accelerationStrength * direction;
            else
                rb.velocity += _accelerationStrength * _airAccelerationFactor * direction;

        }
        else if(context.canceled)
            isMoving = false;

        //animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        //animator.SetFloat("VelocityY", rb.velocity.y);
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
            //Time.timeScale = timeScaleValue;
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScaleValue, 0.2f);
            pfContainer.GetComponent<PFUIContainer>().AnimeUI();
        }
        else
        {
            pfContainer.SetActive(false);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.2f);
            //Time.timeScale = 1f;
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
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, .5f, Vector2.down, groundedSize, ~LayerMask.GetMask("Bouncing Platform"));

        if (hit)
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + groundedSize * Vector2.down, .5f, Color.green, Time.fixedDeltaTime);
        else
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + groundedSize * Vector2.down, .5f, Color.red, Time.fixedDeltaTime);

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
