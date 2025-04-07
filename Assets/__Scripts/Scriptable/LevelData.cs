using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class LevelData : ScriptableObject
{
    public float tickInitial = 0.6f;
    public float tickFastest = 0.01f;
    public float startPosX = -8f;
    public float startPosY = 7.5f;
    public float screenEdge = 16.5f;
    public int descentSteps = 2;
    public float[] invaderShootInterval = { 0.5f, 2.0f };
}
