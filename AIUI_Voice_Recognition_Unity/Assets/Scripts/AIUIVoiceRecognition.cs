using System;
using UnityEngine;
using UnityEngine.Android;

public class AIUIVoiceRecognition
{
    AndroidJavaObject m_AiuiInstance;
    bool _InstanceIsNull
    {
        get
        {
            if (m_AiuiInstance == null)
            {
                Debug.LogError("AIUI Instance is null !");
                return true;
            }
            else
                return false;
        }
    }
    /// <summary>
    /// �ж��Ƿ��Ѵ���Agent
    /// </summary>
    public bool IsCreatedAIUIAgent
    {
        get
        {
            if (!_InstanceIsNull)
            {
                return m_AiuiInstance.Call<bool>("isCreatedAIUIAgent");
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
        if (m_AiuiInstance == null)
            Debug.LogError("AIUI init failed!");
        Debug.Log("AIUI--InitAIUI execute over");
    }
    /// <summary>
    /// ����ʵ��
    /// </summary>
    void GenerateInstance()
    {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        m_AiuiInstance = new AndroidJavaObject("com.wobok.aiui_sdk.AIUIVoiceRecognition", activity);

        BindEvent();
        Debug.Log("AIUI--GenerateInstance execute over");
    }
    /// <summary>
    /// ��״̬�ص�
    /// </summary>
    void BindEvent()
    {
        IRecognitionState recognitionState = new IRecognitionState();
        m_AiuiInstance.Call("setStateCallBack", recognitionState);
        recognitionState.OnWakeupEvent = () => OnWakeup?.Invoke();
        recognitionState.OnSleepEvent = () => OnSleep?.Invoke();
        recognitionState.OnStartRecordEvent = () => OnStartRecordText?.Invoke();
        recognitionState.OnRecordTextEvent = text => OnRecordText?.Invoke(text);
        recognitionState.OnStopRecordEvent = () => OnStopRecordText?.Invoke();
        Debug.Log("AIUI--BindEvent execute over");
    }
    /**************************************************************************************************************************************************/

    /**************************************************************************************************************************************************/
    /// <summary>
    /// ʹ������ʶ��ǰ���ȴ���Agent
    /// </summary>
    public void CreateAgent()
    {
        if (_InstanceIsNull) return;
        m_AiuiInstance.Call("createAgent");
        Debug.Log("AIUI--CreateAgent execute over");
    }
    public void DestoryAgent()
    {
        if (_InstanceIsNull) return;
        m_AiuiInstance.Call("destroyAgent");
        Debug.Log("AIUI--DestoryAgent execute over");
    }
    public void StartRecord()
    {
        if (_InstanceIsNull) return;
        m_AiuiInstance.Call("startRecord");
        Debug.Log("AIUI--StartRecord execute over");
    }
    public void StopRecord()
    {
        if (_InstanceIsNull) return;
        m_AiuiInstance.Call("stopRecord");
        Debug.Log("AIUI--StopRecord execute over");
    }
    /**************************************************************************************************************************************************/

}

class IRecognitionState : AndroidJavaProxy
{
    public Action<string> OnRecordTextEvent;
    public Action OnStartRecordEvent;
    public Action OnStopRecordEvent;
    public Action OnWakeupEvent;
    public Action OnSleepEvent;
    public IRecognitionState() : base("com.wobok.aiui_sdk.util.RecognitionStateCallback")
    {

    }
    void onWakeup()
    {
        OnWakeupEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnWakeup execute over");
    }
    void onSleep()
    {
        OnSleepEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnSleep execute over");
    }
    void onStartRecord()
    {
        OnStartRecordEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnStartRecord execute over");
    }
    void onRecordText(string text)
    {
        OnRecordTextEvent?.Invoke(text);
        Debug.Log("AIUI--IRecognitionEvent--OnRecordTextEvent execute over");
        Debug.Log("AIUI--IRecognitionEvent--����ʶ����: " + text);
    }
    void onStopRecord()
    {
        OnStopRecordEvent?.Invoke();
        Debug.Log("AIUI--IRecognitionEvent--OnStopRecord execute over");
    }
}