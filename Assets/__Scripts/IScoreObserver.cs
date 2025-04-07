using UnityEngine;

public interface IScoreObserver
{
    public void OnScoreChanged(int score);
    public void OnHighScoreChanged(int highscore);
}
