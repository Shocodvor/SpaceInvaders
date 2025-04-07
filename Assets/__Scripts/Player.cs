using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] LaserPool laserPool;
    [SerializeField] float laserDelay;

    Rigidbody2D rb;
    Animator animator;
    float moveX;
    float nextLaserTime;
    bool isShipDestroied;
    float shipWidth;
    Vector2 screenBounds;
    float posY;

    public static Action OnShipDestoried;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        shipWidth = GetComponent<Collider2D>().bounds.extents.x;
        posY = transform.position.y;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    void Update()
    {
        ShipMove();
    }

    void ShipMove()
    {
        if (isShipDestroied)
        {
            rb.linearVelocityX = 0;
            return;
        }

        rb.linearVelocityX = moveX * moveSpeed;

        if (transform.position.x < -screenBounds.x + shipWidth)
        {
            moveTo(new Vector2(-screenBounds.x + shipWidth, posY));
        }
        else if (transform.position.x > screenBounds.x - shipWidth)
        {
            moveTo(new Vector2(screenBounds.x - shipWidth, posY));
        }
    }

    void moveTo(Vector2 pos)
    {
        transform.position = pos;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isShipDestroied)
        {
            moveX = 0;
            return;
        }
        moveX = context.ReadValue<Vector2>().x;
        
    }

    public void FireLaser(InputAction.CallbackContext context)
    {
        if (isShipDestroied)
        {
            return;
        }
        if (context.performed)
        {
            if (Time.time > nextLaserTime)
            {
                SoundManager.Play("Laser");
                nextLaserTime = Time.time + laserDelay;
                GameObject laser = laserPool.LaserGet();
                laser.transform.position = transform.position;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Missile"))
        {
            StartCoroutine(ShipDestoried());
        }
    }

    IEnumerator ShipDestoried()
    {
        isShipDestroied = true;
        SoundManager.Play("ShipKilled");
        animator.SetBool("isShipDestoried", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("isShipDestoried", false);
        isShipDestroied = false;
        OnShipDestoried?.Invoke();
    }
}
