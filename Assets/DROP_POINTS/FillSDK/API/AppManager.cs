using System;
using System.Collections;
using UnityEngine;

public class AppManager : Singleton<AppManager>
{
   // public static Action<int> OnTicketUpdated;
    public static Action<bool> OnGameStarted;
    public static Action<int> OnGameOver;

    protected override void Setup()
    {
        //TelegramApiController.OnAppInitialized += TelegramApiControllerOnOnAppInitialized;
        OnGameOver += FinishSession;
        Debug.Log("AppManager setup");
    }
    public void StartSession()
    {
        Debug.Log("StartSession");
#if UNITY_WEBGL && !UNITY_EDITOR
        TelegramApiController.SendMessageStartSession();
#else
        StartSessionCallback(true);
#endif
    }

    public void StartSessionCallback(bool result)
    {
        Debug.Log("StartSessionCallback");
        //Start game here
        OnGameStarted?.Invoke(result);
    }

    public void FinishSession(int value)
    {
        Debug.Log("FinishSession");
#if UNITY_WEBGL && !UNITY_EDITOR
        TelegramApiController.SendMessageFinishSession(value.ToString());
#else
        FinishSessionCallback(true);
#endif

        //Finish game here
    }
    
    public void FinishSessionCallback(bool result)
    {
        Debug.Log("FinishSessionCallback");
    }
}