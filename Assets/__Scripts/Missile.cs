using UnityEngine;
using UnityEngine.Events;

public class Missile : MonoBehaviour
{
    [SerializeField] float speed = 3.0f;
    float screenHeight;
    public static UnityAction<GameObject> OnRecycleMissle;

    void Start()
    {
        screenHeight = Camera.main.orthographicSize;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -screenHeight)
        {
            OnRecycleMissle!.Invoke(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Bunker":
                OnRecycleMissle?.Invoke(gameObject);
                Destroy(other.gameObject);
                break;

            case "Player":
            case "Laser":
                OnRecycleMissle?.Invoke(gameObject);
                break;

            default:
                Debug.Log(other);
                break;
        }
    }
}
