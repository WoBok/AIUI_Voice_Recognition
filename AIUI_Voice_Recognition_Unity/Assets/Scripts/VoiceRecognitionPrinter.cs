using TMPro;
using UnityEngine;

public class VoiceRecognitionPrinter : MonoBehaviour
{
    public TMP_Text recognitionResult;
    public GameObject recordState;
    public TMP_Text readPoemResult;
    AIUIVoiceRecognition m_VoiceRecognition;
    void Start()
    {
        m_VoiceRecognition = new AIUIVoiceRecognition();
        m_VoiceRecognition.OnRecordText = PrintRecordedText;
        m_VoiceRecognition.OnStartRecordText = OnStartRecord;
        m_VoiceRecognition.OnStopRecordText = OnStopRecord;
        m_VoiceRecognition.OnWakeup = OnWakeup;
        m_VoiceRecognition.OnSleep = OnSleep;
    }
    public void StartRecordText()
    {
        if (!m_VoiceRecognition.IsCreatedAIUIAgent)
            m_VoiceRecognition.CreateAgent();
        m_VoiceRecognition.StartRecord();
    }
    void PrintRecordedText(string text)
    {
        recognitionResult.text = "ʶ������" + text;
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