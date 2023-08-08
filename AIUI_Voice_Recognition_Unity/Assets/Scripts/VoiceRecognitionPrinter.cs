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
        this.recognitionResult.text = "ʶ������" + text;
        Debug.Log("AIUI--VoiceRecognitionPrinter ����ʶ����" + text);
        if (text.Replace(" ", "") == "ɽ��ľ��ľ��֦")
        {
            readPoemResult.text = "��ȷ��";
        }
        else
        {
            readPoemResult.text = "����";
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