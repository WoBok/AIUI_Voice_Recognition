package com.wobok.aiui_sdk.util;

public interface RecognitionStateCallback {
    void onWakeup();

    void onSleep();

    void onStartRecord();

    void onRecordText(String recordedText);

    void onStopRecord();
}
