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
        recognitionResult.text = "识别结果：" + text;
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