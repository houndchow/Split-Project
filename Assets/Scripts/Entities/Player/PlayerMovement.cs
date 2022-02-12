using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Lib;
using Assets.Scripts;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Configurable values
    public float speed = 5f;
    public float gravity = -9f;
    public float jumpSpeed = 300;

    public HealthBar healthBar;
    public HealthBar sanityBar;
    public HealthComponent hc;
    public HealthComponent sc;

    public Countdown sanityHeal;

    public float maxSpeed = 5f;

    public CollisionTypeDetect CTD;

    private Camera playerCamera;

    public LayerMask enemy;
    public Transform punchOrigin;

    public ActionWithCooldown jumpAction;
    public ActionWithCooldown punchAction;

    public Countdown invulTime;

    public bool WeaponOut = true;

    private Rigidbody2D playerBody;

    private DamageFlash flashComp;

    public AudioSource punchSound;

    // Start is called before the first frame update
    void Start()
    {
        jumpAction = new ActionWithCooldown(0.1f, 0.05f, this.Jump);
        punchAction = new ActionWithCooldown(0.0f, 0.5f, this.Punch);

        playerBody = GetComponent<Rigidbody2D>();
        CTD = GetComponent<CollisionTypeDetect>();
        playerCamera = Camera.main;
        punchSound = GetComponent<AudioSource>();
        flashComp = GetComponent<DamageFlash>();

        sanityHeal = new Countdown(0.1f);
        invulTime = new Countdown(0.5f);
    }

    bool Punch()
    {
        punchOrigin.LookAt(playerCamera.ScreenToWorldPoint(Input.mousePosition));
        Vector2 mousePos = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, punchOrigin.forward, 2, enemy);
        if (hit)
        {
            punchSound.Play();
            Debug.Log(hit.transform.name);
            hit.transform.gameObject.GetComponent<Enemy>().TakeDamage(20);
        }
        return true;
    }

    bool Jump()
    {
        if (CTD.IsGrounded)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, 0);
            Vector2 vel2 = new Vector2(0, jumpSpeed);
            playerBody.AddForce(vel2);
            return true;
        }
        return false;
    }

    public void Pull()
    {
        playerBody.AddForce(Vector2.left * 50f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Die();
        }
        if (collision.gameObject.layer == 9)
        {
            Die();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        punchOrigin.LookAt(playerCamera.ScreenToWorldPoint(Input.mousePosition));
        // Determine whether the player is touching something

        if ((Input.GetAxisRaw("Vertical") > 0) ^ Input.GetKey(KeyCode.Space))
        {
            jumpAction.Trigger();
        }
        if (Input.GetMouseButtonDown(0))
        {
            punchAction.Trigger();
        }

        punchAction.Proceed(Time.deltaTime);
        jumpAction.Proceed(Time.deltaTime);

        // Player controlled horizontal force
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector2 velocity = new Vector2(0, 0);
            if (CTD.SlopeLeft)
            {
                velocity = new Vector2(-speed * Time.deltaTime, 0);
                velocity.y = -velocity.x;
                velocity.x = 0;
            }
            else if (!CTD.IsLefted)
            {
                velocity = new Vector2(-speed * Time.deltaTime, 0);
            }
            playerBody.AddForce(1000 * velocity);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector2 velocity = new Vector2(0, 0);
            if (CTD.SlopeRight)
            {
                velocity = new Vector2(speed * Time.deltaTime, 0);
                velocity.y = velocity.x;
                velocity.x = 0;
            }
            else if (!CTD.IsRighted)
            {
                velocity = new Vector2(speed * Time.deltaTime, 0);
            }
            playerBody.AddForce(1000 * velocity);

        }

        // Auto-jump up slopes

        // Max speed

        if (Input.GetKeyDown(KeyCode.E)){
            WeaponOut=!WeaponOut;
        }
        // Crouching
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            speed = 1f;
            maxSpeed = 2f;
        }
        else
        {
            speed = 3f;
            maxSpeed = 5f;
        }

        var newXVel = Math.Max(-maxSpeed, playerBody.velocity.x);
        newXVel = Math.Min(maxSpeed, newXVel);
        playerBody.velocity = new Vector2(newXVel, playerBody.velocity.y);

        sanityHeal.Proceed(Time.deltaTime);
        if (!sanityHeal.IsRunning())
        {
            sc.Heal(1);
            sanityHeal = new Countdown(0.1f);
            sanityHeal.Start();
            sanityBar.UpdateHealth(sc.GetHealthFraction());
        }

        invulTime.Proceed(Time.deltaTime);
    }

    public void SetHealth(int health)
    {
        hc.SetHealth(health);
        healthBar.UpdateHealth(hc.GetHealthFraction());
    }

    public void TakeDamage(float damage)
    {
        if (!invulTime.IsRunning())
        {
            flashComp.Do();
            hc.OnDamage((int)damage);
            healthBar.UpdateHealth(hc.GetHealthFraction());
            if (hc.IsDead())
            {
                Die();
            }
            invulTime.Start();
        }
    }

    public void DamageSanity(float damage)
    {
        /*sc.OnDamage((int)damage);
        sanityHeal = new Countdown(3f);
        sanityHeal.Start();
        sanityBar.UpdateHealth(sc.GetHealthFraction());
        if (sc.IsDead())
        {
            Die();
        }*/
    }

    public void Die()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("player ded");
    }
}
