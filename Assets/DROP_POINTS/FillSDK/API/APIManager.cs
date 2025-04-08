using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class APIManager : Singleton<APIManager>
{
    public UnityEvent OnError;

    public string Prod = "https://api.capsgame.xyz/";
    public string Deb = "https://caps-backend-dev-mrotz.ondigitalocean.app/";

    private string GetUrl()
    {
       if (TelegramApiController.AppData.Enviroment == "DEBUG")
           return Prod;

        return Prod;
    }

    public IEnumerator GetInfo(Action<User> callBack)
    {
        string url = $"{GetUrl()}user/me";
      

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            //webRequest.SetRequestHeader("Authorization", AppKitController.AppData.Token);
            webRequest.SetRequestHeader("x-telegram-id", $"{TelegramApiController.AppData.TelegramId}");

            yield return webRequest.SendWebRequest();

            if ((webRequest.isNetworkError || webRequest.isHttpError) && webRequest.responseCode != 451)
            {
                Debug.LogError("Error: " + webRequest.error);
                ErrorMessage errorMessage =
                    JsonConvert.DeserializeObject<ErrorMessage>(webRequest.downloadHandler.text);
                Debug.LogError($"Error: Code:{errorMessage.error.code} Message:{errorMessage.error.message}");
                OnError?.Invoke();
            }
            else
            {
                User user = JsonConvert.DeserializeObject<User>(webRequest.downloadHandler.text);
                Debug.Log($"GetInfo tickets: {user.Tickets}");
                callBack?.Invoke(user);
            }
        }
    }

    public IEnumerator ClaimReward(int amount, Action callBack)
    {
        string url = $"{GetUrl()}user/claim";
        Debug.Log("Send Reward to server"  + amount);


        var requestData = new Reward()
        {
            Amount = amount,
            GameType = "tournament"
        };
        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            //webRequest.SetRequestHeader("Authorization", AppKitController.AppData.Token);
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("x-telegram-id", $"{TelegramApiController.AppData.TelegramId}");


            yield return webRequest.SendWebRequest();

            if ((webRequest.isNetworkError || webRequest.isHttpError) && webRequest.responseCode != 451)
            {
                Debug.LogError("Error: " + webRequest.error);
                ErrorMessage errorMessage =
                    JsonConvert.DeserializeObject<ErrorMessage>(webRequest.downloadHandler.text);
                Debug.LogError($"Error: Code:{errorMessage.error.code} Message:{errorMessage.error.message}");
                OnError?.Invoke();
            }
            else
            {
                Debug.Log("ClaimReward completed");
                callBack?.Invoke();

              
            }
        }
    }

    public IEnumerator PayTicket(Action<bool> callBack)
    {
        string url = $"{GetUrl()}user/use-ticket";
        Debug.Log("Send PayTicket request to server");

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            //webRequest.SetRequestHeader("Authorization", AppKitController.AppData.Token);
            webRequest.SetRequestHeader("x-telegram-id", $"{TelegramApiController.AppData.TelegramId}");

            yield return webRequest.SendWebRequest();

            if ((webRequest.isNetworkError || webRequest.isHttpError) && webRequest.responseCode != 451)
            {
                if (webRequest.responseCode == 405)
                {
                    Debug.Log("NO");
                    callBack?.Invoke(false);
                    yield break;
                }

                Debug.LogError("Error: " + webRequest.error);
                ErrorMessage errorMessage =
                    JsonConvert.DeserializeObject<ErrorMessage>(webRequest.downloadHandler.text);
                Debug.LogError($"Error: Code:{errorMessage.error.code} Message:{errorMessage.error.message}");
                OnError?.Invoke();
            }
            else
            {
            //    Debug.Log("PayTicket completed");
                User user = JsonConvert.DeserializeObject<User>(webRequest.downloadHandler.text);
               // AppManager.OnTicketUpdated.Invoke(user.Tickets);
                callBack?.Invoke(true);
            }
        }
    }

    [Serializable]
    public class User
    {
        // public string Id { get; set; }
        // public long TelegramID { get; set; }
        // public string UserName { get; set; }
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
        // public int DailyCheckups { get; set; }
        // public DateTime CreatedAt { get; set; }
        // public string Avatar { get; set; }
        // public bool Banned { get; set; }
        // public Balance Balance { get; set; }
        // public List<object> AppliedQuests { get; set; }
        // public List<object> CompletedQuests { get; set; }
        // public List<object> ClaimedQuests { get; set; }
        [JsonProperty("tickets")] public int Tickets { get; set; }
        //  public Activity Activity { get; set; }
        //  public Notifications Notifications { get; set; }
        // public int V { get; set; }
    }

    public class Balance
    {
        public int Main { get; set; }
        public int Referral { get; set; }
        public int Subreferral { get; set; }
        public int Burned { get; set; }
        public string Id { get; set; }
    }

    public class Activity
    {
        public DateTime LastReferralClaim { get; set; }
        public DateTime LastActivity { get; set; }
        public string Id { get; set; }
    }

    public class Notifications
    {
        public string Id { get; set; }
    }

    [Serializable]
    public class Reward
    {
        [JsonProperty("amount")] public int Amount { get; set; }
        [JsonProperty("type")] public string GameType { get; set; }
    }

    [Serializable]
    public class ErrorMessage
    {
        public Error error;
    }

    [Serializable]
    public class Error
    {
        public string code;
        public string message;
    }


    public static double GetCurrentTimestamp()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (DateTime.UtcNow - epochStart).TotalSeconds;
    }
}