using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PFenum
{
    Basic,
    Bounce,
    Boost,
    Lazer,
    Mirror,
    Gravity
}
public class Player2DController : MonoBehaviour
{
    public static Player2DController instance;
    private PFenum choosenPF;
    public float life;
    public float damage;
    [Header("Stats")]
    [SerializeField] private float maxlife;
    [SerializeField] private float speed, minSpeed, MaxSpeed;
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

    }
    public void SelectPF(int id)
    {
        switch (id)
        {
            case 1:choosenPF=PFenum.Basic;break;
            case 2:choosenPF=PFenum.Bounce;break;
            case 3:choosenPF=PFenum.Gravity;break;
            case 4:choosenPF=PFenum.Lazer;break;
            case 5:choosenPF=PFenum.Mirror;break;
            case 6:choosenPF=PFenum.Boost;break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(directionX * speed * Time.deltaTime, rb.velocity.y);
        isGrounded = GroundCheck();
        
    }
    public void MovePlayer(InputAction.CallbackContext context)
    {

            directionX = context.ReadValue<Vector2>().x;
            animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
            animator.SetFloat("VelocityY", rb.velocity.y);
            if (rb.velocity.x < 0)
                sprite.flipX = true;
            else
                sprite.flipX = false;

        
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
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(col.bounds.max.x*0.5f, col.bounds.min.y), -gameObject.transform.up, 0.2f);
        Debug.DrawRay(new Vector2(col.bounds.max.x * 0.5f, col.bounds.min.y), -gameObject.transform.up*0.2f, Color.red); ;
        return hit;
    }
    public void jump(InputAction.CallbackContext context)
    {   if(isGrounded)
            if(context.performed)
                _jumpCoroutine= StartCoroutine(JumpCoroutine());
            else if(context.canceled)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
                rb.gravityScale = 1;
            }
    }
    private IEnumerator JumpCoroutine()
    {

        float t = 0;
        rb.gravityScale = 0;
        while (t < 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.LerpUnclamped(jumpForce, 0, jumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / jumpDuration;
            yield return new WaitForFixedUpdate();
        }
        rb.gravityScale = 1;
        yield break;
    }

}
