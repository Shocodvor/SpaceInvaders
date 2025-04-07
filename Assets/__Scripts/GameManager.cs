using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderGroupInfo
{
    public float leftX;
    public float rightX;
    public float topY;
    public float bottomY;
    public int invaders;
    public int sprite;
}

public class GameManager : MonoBehaviour
{
    [Header("--- Level data ---")]
    [SerializeField] LevelData[] levels;

    [Header("--- Pools ---")]
    [SerializeField] MissilePool missilePool;
    [SerializeField] InvaderPool invaderPool;

    [Header("--- Level data cache ---")]
    [SerializeField] float startPosX = -8f;
    [SerializeField] float startPosY = 7.5f;
    [SerializeField] float spacingX = 2f;
    [SerializeField] float spacingY = -1.5f;
    [SerializeField] float moveAmountX = 0.25f;
    [SerializeField] float moveAmountY = 0.5f;
    [SerializeField] float tickInitial = 0.5f;
    [SerializeField] float tick = 0.5f;
    [SerializeField] float tickFastest = 0.01f;
    [SerializeField] float screenEdge = 16f;
    [SerializeField] float deadline = -8f;
    int invaderDirection = 1;
    int descentSteps = 2;
    int descentCurrentSteps;
    bool invaderDescent;
    public bool InvaderCanShoot;
    float[] invaderShootInterval = { 0.5f, 2.0f };
    float invaderShootNextTime;

    [Header("--- Global game variables ---")]
    [SerializeField] int life = 3;
    [SerializeField] int score = 0;
    [SerializeField] int highScore = 0;
    [SerializeField] GameObject player;

    [HideInInspector]
    public List<Vector2> missileStartPos = new List<Vector2>();

    string[] tickSounds = { "Tick0", "Tick1", "Tick2", "Tick3" };
    int tickSoundsIndex = 0;

    public static Action<InvaderGroupInfo, Vector3> InvaderMove;
    public static Action OnGameStart;
    public static Action<int> OnLifeChanged;
    public static Action OnGameOver;
    InvaderGroupInfo info;
    public int Level = 0;
    public static bool isGameRunning = false;
    public static GameManager Instance {  get; private set; }

    List<IScoreObserver> observers = new List<IScoreObserver>();

    void Awake()
    {
        Instance = this;
        Player.OnShipDestoried += OnShipDestoried;
        Invader.OnInvaderDead += OnInvaderDead;
    }

    void Start()
    {
        info = new InvaderGroupInfo();
        info.leftX = startPosX;
        info.rightX = startPosX;
        info.topY = startPosY;
        info.bottomY = startPosY;

        UnityEngine.Random.InitState((int)Time.time);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        ObserverNotifyHighScore();
        GameStart();
    }

    void OnDisable()
    {
        Invader.OnInvaderDead -= OnInvaderDead;
        Player.OnShipDestoried -= OnShipDestoried;
    }

    void GameStart()
    {
        OnGameStart!.Invoke();
    }

    void OnShipDestoried()
    {
        if (life > 0)
        {
            OnLifeChanged.Invoke(--life);
        }
        else
        {
            GameOver();
        }
    }

    public void ObserverAdd(IScoreObserver observer)
    {
        observers.Add(observer);
    }

    public void ObserverRemove(IScoreObserver observer)
    {
        observers.Remove(observer);
    }

    public void ObserverNotifyScore()
    {
        foreach (var observer in observers)
        {
            observer.OnScoreChanged(score);
        }
    }

    public void ObserverNotifyHighScore()
    {
        foreach (var observer in observers)
        {
            observer.OnHighScoreChanged(highScore);
        }
    }

    void OnInvaderDead(int invaderScore)
    {
        info.invaders--;
        CalculateTick();
        ScoreAdd(invaderScore);
    }

    public void ScoreAdd(int scoreToAdd)
    {
        score += scoreToAdd;
        ObserverNotifyScore();
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            ObserverNotifyHighScore();
        }
    }

    public void LevelStart()
    {
        UIManager.Instance.LevelDisplay(Level+1);
        OnLifeChanged.Invoke(life);
        player.SetActive(true);
        LevelInit();
    }

    void LevelInit()
    {
        int levelIndex = Level;

        levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        tickInitial = levels[levelIndex].tickInitial;
        tickFastest = levels[levelIndex].tickFastest;
        startPosY = levels[levelIndex].startPosY;
        screenEdge = levels[levelIndex].screenEdge;
        descentSteps = levels[levelIndex].descentSteps;
        invaderShootInterval = levels[levelIndex].invaderShootInterval;

        info.leftX = startPosX;
        info.rightX = startPosX;
        info.topY = startPosY;
        info.bottomY = startPosY;
        info.invaders = 0;
        info.sprite = 0;

        invaderDirection = 1;
        descentCurrentSteps = 0;
        invaderDescent = false;
        InvaderCanShoot = false;

        UFO.UFOReset();
        InitializeInvaders();
        StopAllCoroutines();
        StartCoroutine(GameProcess());
    }

    void InitializeInvaders()
    {
        int invaderType = 0;
        for (int y = 0; y < 5; y++)
        {
            if (y == 1 || y == 3)
            {
                invaderType++;
            }
            for (int x = 0; x < 10; x++)
            {
                info.invaders++;
                var invader = invaderPool.InvaderGet(InvaderPool.Instance.invaderDatas[invaderType].prefab);
                invader.transform.position = new Vector3(startPosX + spacingX * x, startPosY + spacingY * y, 0);
            }
        }
        CalculateTick();
    }

    void CalculateTick()
    {
        tick = Mathf.Lerp(tickFastest, tickInitial, (float)info.invaders / 50f);
    }

    IEnumerator GameProcess()
    {
        while (isGameRunning)
        {
            yield return new WaitForSeconds(tick);
            InvaderCheckIfCanShoot();
            InvaderProgress();
            InvaderShoot();
            GameOverCheck();
            LevelCompleteCheck();
        }
        GameOver();
    }

    void InvaderCheckIfCanShoot()
    {
        if (Time.time > invaderShootNextTime)
        {
            missileStartPos.Clear();
            InvaderCanShoot = true;
        }
    }

    void InvaderShoot()
    {
        if (missileStartPos.Count > 0 && InvaderCanShoot)
        {
            GameObject missile = missilePool.MissileGet();
            missile.transform.position = missileStartPos[UnityEngine.Random.Range(0, missileStartPos.Count)];
            InvaderCanShoot = false;
            invaderShootNextTime = Time.time + UnityEngine.Random.Range(invaderShootInterval[0], invaderShootInterval[1]);
        }
    }

    void InvaderProgress()
    {
        info.sprite = (info.sprite + 1) % 2;
        PlayTickSound();
        if (invaderDescent == true)
        {
            InvaderVerticalMove();
        }
        else
        {
            InvaderHorizontalMove();
        }
    }

    void InvaderVerticalMove()
    {
        if (descentCurrentSteps == descentSteps)
        {
            invaderDescent = false;
            descentCurrentSteps = 0;
            InvaderHorizontalMove();
        }
        else
        {
            InvaderMove!.Invoke(info, Vector3.down * moveAmountY);
            descentCurrentSteps++;
        }
    }

    void InvaderHorizontalMove()
    {
        if ((info.rightX >= screenEdge && invaderDirection == 1) ||
           (info.leftX <= -screenEdge && invaderDirection == -1))
        {
            invaderDirection *= -1;
            invaderDescent = true;
            InvaderVerticalMove();
        }
        else
        {
            if (InvaderMove != null)
            {
                InvaderMove!.Invoke(info, Vector3.right * moveAmountX * invaderDirection);
            }
        }
    }

    void PlayTickSound()
    {
        SoundManager.Play(tickSounds[tickSoundsIndex]);
        tickSoundsIndex = (tickSoundsIndex + 1) % 4;
    }

    void LevelCompleteCheck()
    {
        if (info.invaders == 0)
        {
            InvaderPool.Instance.InvaderPoolInit();
            LaserPool.Instance.LaserPoolInit();
            MissilePool.Instance.MissilePoolInit();
            UIManager.Instance.LevelDisplay(++Level+1);

            LevelInit();
        }
    }

    void GameOverCheck()
    {
        if (info.bottomY <= deadline)
        {
            isGameRunning = false;
        }
    }

    void GameOver()
    {
        OnGameOver.Invoke();
    }
}
