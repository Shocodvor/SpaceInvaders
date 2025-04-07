using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPool : MonoBehaviour
{
    [SerializeField] GameObject laserPrefab;
    [SerializeField] int poolSize = 10;
    Queue<GameObject> laserPool;
    public static LaserPool Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Laser.OnRecycleLaser += LaserRecycle;
    }

    void Start()
    {
        LaserPoolCreate();
    }

    void OnDestroy()
    {
        Laser.OnRecycleLaser -= LaserRecycle;
    }

    void LaserPoolCreate()
    {
        laserPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var laser = Instantiate(laserPrefab);
            laser.SetActive(false);
            laserPool.Enqueue(laser);
        }
    }

    public void LaserPoolInit()
    {
        var activeItems = new List<GameObject>();

        foreach (var laser in laserPool)
        {
            activeItems.Add(laser);
        }

        foreach (var laser in activeItems)
        {
            LaserRecycle(laser);
        }
    }

    public GameObject LaserGet()
    {
        if (laserPool.Count > 0)
        {
            var laser = laserPool.Dequeue();
            laser.SetActive(true);
            return laser;
        }
        else
        {
            var laser = Instantiate(laserPrefab);
            return laser;
        }
    }

    public void LaserRecycle(GameObject laser)
    {
        if (laser.activeSelf == true)
        {
            laser.SetActive(false);
            laserPool.Enqueue(laser);
        }
    }
}
