using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour,IEntity
{
    public float life { get; set; }
    public float damage { get; set; }
    [Header("Stats")]
    [SerializeField]private float maxlife;
    [SerializeField]private float speed, minSpeed,MaxSpeed;
    [SerializeField] private float coins;
    [SerializeField]private GameObject playerVisual;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = playerVisual.GetComponent<SpriteRenderer>();
        animator = playerVisual.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.velocity = direction.normalized * Time.fixedDeltaTime * speed;
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VelocityY", rb.velocity.y);

        if (rb.velocity.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;
    }
    public void UpdateDirection(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }
    public void interact()
    {

    }
    public void attack()
    {
        
    }
    public void TakeDamage(float damage)
    {
        life -= damage;
    }
    public void GiveHeal(float Heal)
    {
        life += Heal;
    }
}
