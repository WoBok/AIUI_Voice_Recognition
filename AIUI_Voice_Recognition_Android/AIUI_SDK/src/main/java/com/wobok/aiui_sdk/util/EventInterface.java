package com.wobok.aiui_sdk.util;

public interface EventInterface {
    void OnRecordText(String recordedText);

    void OnStartRecord();

    void OnStopRecord();
    void OnWakeup();
    void OnSleep();
}
