using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float moveSpeed = 5f; // vitesse de déplacement horizontale du joueur
    public float jumpForce = 7f; // force du saut
    private Rigidbody2D rb; // le corps rigide du joueur
    private bool isGrounded; // pour vérifier si le joueur est au sol
    private int jumpsLeft; // le nombre de sauts restants que le joueur peut effectuer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = 1;
    }

    void Update()
    {
        // Gestion des mouvements horizontaux
        float moveDirection = Input.GetAxisRaw("Horizontal"); // -1 pour gauche, 1 pour droite
        rb.AddForce(new Vector2(moveDirection * moveSpeed, 0f));

        // Vérifie si le joueur est au sol
        if (isGrounded)
        {
            jumpsLeft = 1;
        }

        // Gestion du saut
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpsLeft > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsLeft--;
            }
        }
    }

    void FixedUpdate()
    {
        // Vérifie si le joueur est au sol
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f);
        if (hit.collider != null && hit.distance < 0.1f)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Gestion de la gravité
        if (!isGrounded)
        {
            rb.AddForce(new Vector2(0f, -30f * rb.mass));
        }
    }
}