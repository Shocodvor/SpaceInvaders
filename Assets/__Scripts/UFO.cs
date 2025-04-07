using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UFO : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float[] UFOInterval;
    float UFONextTime;
    static bool UFOLaunched;
    static bool UFOPreparing;
    [SerializeField] int UFODirection;
    [SerializeField] float UFOPosX;
    [SerializeField] float UFOPosY;
    [SerializeField] float UFOSpeed;
    [SerializeField] int[] UFOScore;
    [SerializeField] GameObject points;
    public static UFO Instance { get; private set; }

    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        UFOLaunched = false;
        UFONextTimeCalculate();
    }

    void Update()
    {
        if (!UFOPreparing && !UFOLaunched)
        {
            StartCoroutine(UFOPrefare());
        }
    }

    void UFOLaunch()
    {
        UFOLaunched = true;
        UFODirection = (Random.value > 0.5) ? -1 : 1;

        transform.position = new Vector3(UFOPosX * UFODirection * -1, UFOPosY, 0);
        SoundManager.Play("UFO");
        StartCoroutine(UFOMove());
    }

    void UFONextTimeCalculate()
    {
        float timeToWait = UnityEngine.Random.Range(UFOInterval[0], UFOInterval[1]);
        UFONextTime = Time.time + timeToWait;
    }

    IEnumerator UFOMove()
    {
        while (UFOLaunched)
        {
            transform.Translate(new Vector3(UFODirection * UFOSpeed * Time.deltaTime, 0, 0));
            float PosX = transform.position.x;
            if (UFODirection == 1 && PosX > UFOPosX ||
                UFODirection == -1 && PosX < -UFOPosX)
            {
                UFOPreparing = false;
                UFOLaunched = false;
                UFONextTimeCalculate();
            }
            yield return null;
        }
    }

    IEnumerator UFOPrefare()
    {
        UFOPreparing = true;
        while (GameManager.isGameRunning && !UFOLaunched)
        {
            if (Time.time > UFONextTime)
            {
                UFOLaunch();
            }
            yield return null;
        }
        UFOPreparing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            UFOKilled();
        }
    }

    private void UFOKilled()
    {
        SoundManager.Play("UFOKilled");
        UFOLaunched = false;
        UFONextTimeCalculate();

        var pointsInstance = Instantiate(points, transform);
        pointsInstance.transform.parent = null;
        transform.position = new Vector3(UFOPosX, UFOPosY, 0);

        int scoreToAdd = UFOScore[Random.Range(0, UFOScore.Length)];
        pointsInstance.GetComponentInChildren<TMP_Text>().text = scoreToAdd.ToString();
        GameManager.Instance.ScoreAdd(scoreToAdd);
    }

    public static void UFOReset()
    {
        UFOPreparing = false;
        UFOLaunched = false;
        Instance.transform.position = new Vector3(Instance.UFOPosX, Instance.UFOPosY, 0);
    }
}
