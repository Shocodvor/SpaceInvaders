using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNativeShare : MonoBehaviour
{
    //Public variables
    public static string _applicationAndroidID = "com.ookamiGames.KpopNews";
    //public static string _applicationAndroidID => Application.identifier;
    public static string _applicationIOSID = "1057997453";

    //Private variables
    private const string _screenshotName = "screenshot.png";


    public static IEnumerator ShareScreenshotWithText(string text)
    {
        string screenShotPath = Application.persistentDataPath + "/" + _screenshotName;
        ScreenCapture.CaptureScreenshot(_screenshotName);
        yield return new WaitForEndOfFrame();

        text += "\n" + GetURLStore();
        Share(text, screenShotPath);
    }

    private static void Share(string shareText, string imagePath, string subject = "Share")
    {
#if UNITY_EDITOR
        Debug.Log("To use the share function, build the game for Android platform.");
#elif UNITY_ANDROID
        //Current activity context.
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        //Create intent for action send.
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        //Create file object of the screenshot captured.
        AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", imagePath);
        
        //Create FileProvider class object.
        AndroidJavaClass fileProviderClass = new AndroidJavaClass("androidx.core.content.FileProvider");
        object[] providerParams = new object[3];
        providerParams[0] = currentActivity;
        providerParams[1] = Application.identifier + ".provider";
        providerParams[2] = fileObject;

        //Get the uri from file using FileProvider.
        AndroidJavaObject uriObject = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", providerParams);

        //Put image and string extra.
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        intentObject.Call<AndroidJavaObject>("setType", "image/png");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

        //Additionally grant permission to read the uri.
        intentObject.Call<AndroidJavaObject>("addFlags", intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));

        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
        currentActivity.Call("startActivity", chooser);
#else
		Debug.Log("The sharing function is not supported for this platform.");
#endif
    }

    public static void Rate()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + _applicationAndroidID);
#elif UNITY_IOS
		Application.OpenURL("itms-apps://itunes.apple.com/app/id"+ _applicationIOSID); 
#else
		Debug.Log("The rate function is not configured for this platform.");
#endif
    }

    public static string GetURLStore()
    {
        string URLStore = "";
#if UNITY_ANDROID
        URLStore = "https://play.google.com/store/apps/details?id=" + _applicationAndroidID;
#elif UNITY_IOS
		URLStore = "https://itunes.apple.com/app/id" + _applicationIOSID;
#endif
        return URLStore;
    }
}
