using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class Invader : MonoBehaviour
{
    public int score;
    public Color color;
    public GameObject prefab;
    public MissilePool missilePool;
    public static Action<GameObject, GameObject> OnRecycleInvader;
    public static Action<int> OnInvaderDead;
    public Sprite[] sprite;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    float rayLength = 3f;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = color;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        GameManager.InvaderMove += InvaderMove;
    }

    void OnDisable()
    {
        GameManager.InvaderMove -= InvaderMove;
    }

    void InvaderMove(InvaderGroupInfo info, Vector3 move)
    {
        transform.Translate(move);
        spriteRenderer.sprite = sprite[info.sprite];
        float myPositionX = transform.position.x;
        float myPositionY = transform.position.y;
        RegisterShootPos();

        if (move.x > 0)
        {
            if (myPositionX > info.rightX)
            {
                info.rightX = myPositionX;
            }
            if (myPositionX > info.leftX)
            {
                info.leftX = myPositionX;
            }
        }
        if (move.x < 0)
        {
            if (myPositionX < info.rightX)
            {
                info.rightX = myPositionX;
            }
            if (myPositionX < info.leftX)
            {
                info.leftX = myPositionX;
            }
        }
        if (myPositionY < info.bottomY)
        {
            info.bottomY = myPositionY;
        }
    }

    void RegisterShootPos()
    {
        if (!GameManager.Instance.InvaderCanShoot)
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.down * 1.0f;
            
        RaycastHit2D ray = Physics2D.Raycast(origin, Vector2.down, rayLength);
        if (ray.collider == null || !ray.collider.CompareTag("Enemy"))
        {
            origin += Vector3.right * (UnityEngine.Random.value > 0.5f ? 0.1f : -0.1f);
            GameManager.Instance.missileStartPos.Add(origin);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.down * 1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLength);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            SoundManager.Play("InvaderKilled");
            OnInvaderDead!.Invoke(score);
            OnRecycleInvader!.Invoke(gameObject, prefab);
        }
        if (other.CompareTag("Bunker"))
        {
            Destroy(other.gameObject);
        }
    }
}
