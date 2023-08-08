using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceRecognitionPrinter : MonoBehaviour
{
    public TMP_Text recognitionResult;
    AIUIVoiceRecognition voiceRecognition;
    public GameObject recordState;
    public TMP_Text readPoemResult;
    void Start()
    {
        voiceRecognition = new AIUIVoiceRecognition();
        voiceRecognition.OnRecordText = PrintRecordedText;
        voiceRecognition.OnStartRecordText = OnStartRecord;
        voiceRecognition.OnStopRecordText = OnStopRecord;
        voiceRecognition.OnWakeup = OnWakeup;
        voiceRecognition.OnSleep = OnSleep;
    }
    public void StartRecordText()
    {
        if (!voiceRecognition.IsCreatedAIUIAgent)
            voiceRecognition.CreateAgent();
        voiceRecognition.StartRecord();
    }
    void PrintRecordedText(string text)
    {
        this.recognitionResult.text = "识别结果：" + text;
        Debug.Log("AIUI--VoiceRecognitionPrinter 语音识别结果" + text);
        if (text.Replace(" ", "") == "山有木兮木有枝")
        {
            readPoemResult.text = "正确！";
        }
        else
        {
            readPoemResult.text = "错误！";
        }
    }
    void OnStartRecord()
    {
        recordState.SetActive(true);
    }
    void OnStopRecord()
    {
        recordState.SetActive(false);
    }
    void OnWakeup()
    {
        recordState.SetActive(true);
    }
    void OnSleep()
    {
        recordState.SetActive(false);
    }
}