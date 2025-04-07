using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class InvaderPool : MonoBehaviour
{
    [SerializeField] MissilePool missilePool;
    public InvaderData[] invaderDatas;
    public int[] poolSize;
    public static InvaderPool Instance {  get; private set; }
    
    Dictionary<GameObject, Queue<GameObject>> invaderPools = 
        new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
        Invader.OnRecycleInvader += InvaderRecycle;
        InvaderPoolCreate();
    }

    void OnDestroy()
    {
        Invader.OnRecycleInvader -= InvaderRecycle;
    }

    void InvaderPoolCreate()
    {
        int typeIndex = 0;

        foreach (var invaderData in invaderDatas)
        {
            var invaderQueue = new Queue<GameObject>();
            for (int i = 0; i < poolSize[typeIndex]; i++)
            {
                invaderPoolPopulate(invaderQueue, invaderData);
            }
            invaderPools.Add(invaderData.prefab, invaderQueue);
            typeIndex++;
        }
    }

    public void InvaderPoolInit()
    {
        foreach (var invaderQueue in invaderPools.Values)
        {
            var activeInvaders = new List<GameObject>();

            foreach (var invader in invaderQueue)
            {
                if (invader.activeSelf)
                {
                    activeInvaders.Add(invader);
                }
            }
            foreach (var invader in activeInvaders)
            {
                invader.SetActive(false);
            }
        }
    }

    void invaderPoolPopulate(Queue<GameObject> invaderQueue, InvaderData invaderData)
    {
        GameObject invaderInstance = Instantiate(invaderData.prefab);
        Invader invader = invaderInstance.GetComponent<Invader>();
        invader.score = invaderData.score;
        invader.color = invaderData.color;
        invader.prefab = invaderData.prefab;
        invader.missilePool = missilePool;
        invaderInstance.SetActive(false);
        invaderQueue.Enqueue(invaderInstance);
    }

    public GameObject InvaderGet(GameObject prefab)
    {
        if (invaderPools.ContainsKey(prefab) && invaderPools[prefab].Count > 0)
        {
            var invader = invaderPools[prefab].Dequeue();
            invader.SetActive(true);
            return invader;
        }
        return null;
    }

    public void InvaderRecycle(GameObject invader, GameObject prefab)
    {
        invader.SetActive(false);
        invaderPools[prefab].Enqueue(invader);
    }
}
