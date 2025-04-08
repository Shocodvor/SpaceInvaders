mergeInto(LibraryManager.library, {
  AppStarted: function () {
    window.dispatchReactUnityEvent("AppStarted");
  },
  
  OnClaimReward: function () {   
    window.dispatchReactUnityEvent("OnClaimReward");
  },
  
  SendMessageStartSession: function() {
    console.log("JS: SendMessageStartSession called");
      window.SendMessageStartSession();
  },
  
  SendMessageFinishSession: function(points) {
   var decodedPoints = UTF8ToString(points);
     console.log("JS: SendMessageFinishSession called");
      window.SendMessageFinishSession(decodedPoints);
  }
}); 