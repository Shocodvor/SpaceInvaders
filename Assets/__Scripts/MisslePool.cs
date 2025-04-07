using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePool : MonoBehaviour
{
    [SerializeField] GameObject missilePrefab;
    [SerializeField] int poolSize = 20;
    Queue<GameObject> missilePool;
    public static MissilePool Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Missile.OnRecycleMissle += MissleRecycle;
    }

    void Start()
    {
        MissilePoolCreate();
    }

    void OnDestroy()
    {
        Missile.OnRecycleMissle -= MissleRecycle;
    }

    private void MissilePoolCreate()
    {
        missilePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var missile = Instantiate(missilePrefab);
            missile.SetActive(false);
            missilePool.Enqueue(missile);
        }
    }

    public void MissilePoolInit()
    {
        var activeItems = new List<GameObject>();

        foreach (var missile in missilePool)
        {
            activeItems.Add(missile);
        }

        foreach (var missile in activeItems)
        {
            MissleRecycle(missile);
        }
    }

    public GameObject MissileGet()
    {
        if (missilePool.Count > 0)
        {
            var missile = missilePool.Dequeue();
            missile.SetActive(true);
            return missile;
        }
        else
        {
            var missle = Instantiate(missilePrefab);
            return missle;
        }
    }

    public void MissleRecycle(GameObject missile)
    {
        if (missile.activeSelf == true)
        {
            missile.SetActive(false);
            missilePool.Enqueue(missile);
        }
    }
}
