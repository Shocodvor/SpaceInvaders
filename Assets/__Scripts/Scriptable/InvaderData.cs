using UnityEngine;

[CreateAssetMenu(fileName = "NewInvader", menuName = "Scriptable Objects/Invader")]
public class InvaderData : ScriptableObject
{
    public int score;
    public Color color;
    public GameObject prefab;
}
