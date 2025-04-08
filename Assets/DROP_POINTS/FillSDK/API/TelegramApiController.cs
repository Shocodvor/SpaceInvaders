using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class TelegramApiController : Singleton<TelegramApiController>
{
    [DllImport("__Internal")]
    private static extern void OnClaimReward();
    public static event Action OnAppInitialized;
    [DllImport("__Internal")]
    public static extern void SendMessageStartSession();
    [DllImport("__Internal")]
    public static extern void SendMessageFinishSession(string points);
    public static bool IsAppInitialised { get; private set; }
    public static AppData AppData { get; private set; }
 

    private bool isReadyToTap;
    private float delay = 0.7f;
    private float touchTime;

    void Start()
    {
        Debug.Log($"Start AppKitController");
        DontDestroyOnLoad(this);
   #if UNITY_EDITOR
        string appData = JsonConvert.SerializeObject(new AppData { TelegramId = 5204406969, Enviroment = "�" });
      //  Debug.Log($"In Editor Initialize");
        //InitializeApp("518055071"); //518055071  118255814
        gameObject.SendMessage("InitializeApp", appData);
    #endif
    }

    public void InitializeApp(string str)
    {
        AppData = JsonConvert.DeserializeObject<AppData>(str);
        Debug.Log($"InitializeApp Telegram id: {AppData.TelegramId} Env: {AppData.Enviroment}");
        //AppData = new AppData{TelegramId = telegramId};
        IsAppInitialised = true;
        OnAppInitialized?.Invoke();
        Debug.Log($"App version : {Application.version}");
    }

    public void StartSessionCallback(string resultData)
    {
        var data = JsonConvert.DeserializeObject<ResultData>(resultData); 
        data.isSuccess = true;
        AppManager.Instance.StartSessionCallback(data.isSuccess);
    }
    
    public void FinishSessionCallback(string resultData)
    {
        var data = JsonConvert.DeserializeObject<ResultData>(resultData);
        data.isSuccess = true;
        AppManager.Instance.FinishSessionCallback(data.isSuccess);
    }
    
}   

[Serializable]
public class AppData
{
    [JsonProperty("telegramId")]
    public long TelegramId { get; set; }
    
    [JsonProperty("env")]//[JsonConverter(typeof(StringEnumConverter))]
    public string Enviroment { get; set; }
}

[Serializable]
public enum Env
{
    DEBUG,
    PRODUCTION
}

[Serializable]
public class ResultData
{
    [JsonProperty("isSuccess")]
    public bool isSuccess;
}