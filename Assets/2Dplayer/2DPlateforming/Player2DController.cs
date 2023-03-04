using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public enum PFenum
{
    Basic=1,
    Bounce=2,
    Boost=3,
    Lazer=4,
    Mirror=5,
    Gravity=6,
    None=0
}
public class Player2DController : MonoBehaviour
{
    public static Player2DController instance;
    private PFenum choosenPF;
    [SerializeField] private GameObject pfContainer;
    public float life;
    public float damage;
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
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpForce = 10f;
    private CapsuleCollider2D col;
    private Coroutine _jumpCoroutine;
    private float duration;
    private float timer;
    [SerializeField] private float fallGravity;

    private Vector2 direction;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    [SerializeField] private float frictionAnmount;
    [SerializeField] private float groundedSize;


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
        //switch (id)
        //{
        //    case 1:choosenPF=PFenum.Basic;break;
        //    case 2:choosenPF=PFenum.Bounce;break;
        //    case 3:choosenPF=PFenum.Gravity;break;
        //    case 4:choosenPF=PFenum.Lazer;break;
        //    case 5:choosenPF=PFenum.Mirror;break;
        //    case 6:choosenPF=PFenum.Boost;break;
        //    default:
        //        break;
        //}
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
            Time.timeScale = 0.2f;
            //pfContainer.GetComponent<PFUIContainer>().AnimeUI();
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
    public void jump(InputAction.CallbackContext context)
    {   if (GroundCheck())
            if (context.performed)
                _jumpCoroutine= StartCoroutine(JumpCoroutine());
            else if (context.canceled)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
                rb.gravityScale = 1;
            }
    }
    private IEnumerator JumpCoroutine()
    {
        //isJumping = true;
        
        float t = 0;
        rb.gravityScale = 0;
        while (t < 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.LerpUnclamped(jumpForce, 0, jumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / jumpDuration;
            yield return new WaitForFixedUpdate();
        }
        //playerSystemManager.isJumping = false;
        //playerSystemManager.Rb2D.gravityScale = playerSystemManager.GravityScale;
        rb.gravityScale = fallGravity;
        _jumpCoroutine = null;
        yield break;
    }

}
