using System;
using UnityEngine;
using UnityEngine.Android;

public class AIUIVoiceRecognition
{
    AndroidJavaObject aiuiInstance;
    bool InstanceIsNull
    {
        get
        {
            if (aiuiInstance == null)
            {
                Debug.LogError("AIUI Instance is null !");
                return true;
            }
            else
                return false;
        }
    }
    public bool IsCreatedAIUIAgent
    {
        get
        {
            if (!InstanceIsNull)
            {
                return aiuiInstance.Call<bool>("IsCreatedAIUIAgent");
            }
            return false;
        }
    }
    public Action OnWakeup;
    public Action OnSleep;
    public Action OnStartRecordText;
    public Action<string> OnRecordText;
    public Action OnStopRecordText;
    /**************************************************************************************************************************************************/
    public AIUIVoiceRecognition()
    {
        InitAIUI();
    }
    void InitAIUI()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            GenerateInstance();
        else
        {
            PermissionCallbacks callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += (value) =>
            {
                GenerateInstance();
                Debug.Log("AIUI--Permission Granted: " + value);
            };
            callbacks.PermissionDenied += (value) =>
            {
                Debug.LogError("AIUI--Permission Denied: " + value);
            };
            callbacks.PermissionDeniedAndDontAskAgain += (value) =>
            {
                Debug.LogError("AIUI--Permission Denied And Dont Ask Again: " + value);
            };
            Permission.RequestUserPermission(Permission.Microphone, callbacks);
        }
        if (aiuiInstance == null)
            Debug.LogError("AIUI init failed!");
        Debug.Log("AIUI--InitAIUI execute over");
    }
    void GenerateInstance()
    {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        aiuiInstance = new AndroidJavaObject("com.wobok.aiui_sdk.AIUIVoiceRecognition", activity);

        BindEvent();
        Debug.Log("AIUI--GenerateInstance execute over");
    }
    void BindEvent()
    {
        IRecognitionEvent recognitionEvent = new IRecognitionEvent();
        aiuiInstance.Call("SetEventCallBack", recognitionEvent);
        recognitionEvent.OnWakeupEvent = () => OnWakeup?.Invoke();
        recognitionEvent.OnSleepEvent = () => OnSleep?.Invoke();
        recognitionEvent.OnStartRecordEvent = () => OnStartRecordText?.Invoke();
        recognitionEvent.OnRecordTextEvent = text => OnRecordText?.Invoke(text);
        recognitionEvent.OnStopRecordEvent = () => OnStopRecordText?.Invoke();
        Debug.Log("AIUI--BindEvent execute over");
    }
    /**************************************************************************************************************************************************/


    /**************************************************************************************************************************************************/
    public void CreateAgent()
    {
        if (InstanceIsNull) return;
        aiuiInstance.Call("CreateAgent");
        Debug.Log("AIUI--CreateAgent execute over");
    }
    public void DestoryAgent()
    {
        if (InstanceIsNull) return;
        aiuiInstance.Call("DestroyAgent");
        Debug.Log("AIUI--DestoryAgent execute over");
    }
    public void StartRecord()
    {
        if (InstanceIsNull) return;
        aiuiInstance.Call("StartRecord");
        Debug.Log("AIUI--StartRecord execute over");
    }
    public void StopRecord()
    {
        if (InstanceIsNull) return;
        aiuiInstance.Call("StopRecord");
        Debug.Log("AIUI--StopRecord execute over");
    }
    /**************************************************************************************************************************************************/
}

class IRecognitionEvent : AndroidJavaProxy
{
    public Action<string> OnRecordTextEvent;
    public Action OnStartRecordEvent;
    public Action OnStopRecordEvent;
    public Action OnWakeupEvent;
    public Action OnSleepEvent;
    public IRecognitionEvent() : base("com.wobok.aiui_sdk.util.EventInterface")
    {

    }
    void OnWakeup()
    {
        OnWakeupEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnWakeup execute over");
    }
    void OnSleep()
    {
        OnSleepEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnSleep execute over");
    }
    void OnStartRecord()
    {
        OnStartRecordEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnStartRecord execute over");
    }
    void OnRecordText(string text)
    {
        OnRecordTextEvent?.Invoke(text);
        Debug.Log("AIUI--IRecognitionEvent--OnRecordTextEvent execute over");
        Debug.Log("AIUI--IRecognitionEvent--语音识别结果: " + text);
    }
    void OnStopRecord()
    {
        OnStopRecordEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnStopRecord execute over");
    }
}