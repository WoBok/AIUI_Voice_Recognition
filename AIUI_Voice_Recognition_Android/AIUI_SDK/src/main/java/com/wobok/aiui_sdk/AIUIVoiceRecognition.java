package com.wobok.aiui_sdk;

import android.app.Activity;
import android.content.Context;
import android.content.res.AssetManager;
import android.text.TextUtils;
import android.util.Log;

import com.iflytek.aiui.AIUIAgent;
import com.iflytek.aiui.AIUIConstant;
import com.iflytek.aiui.AIUIEvent;
import com.iflytek.aiui.AIUIListener;
import com.iflytek.aiui.AIUIMessage;
import com.iflytek.aiui.AIUISetting;
import com.wobok.aiui_sdk.util.DeviceUtils;
import com.wobok.aiui_sdk.util.EventInterface;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;

public class AIUIVoiceRecognition {
    static String TAG = "AIUI";
    Context context = null;
    AIUIAgent mAIUIAgent = null;
    int mAIUIState = AIUIConstant.STATE_IDLE;
    EventInterface eventInterface;

    public AIUIVoiceRecognition(Activity activity) {
        context = activity;
    }

    public void CreateAgent() {
        if (null == mAIUIAgent) {
            Log.i(TAG, "创建AIUIAgent");

            String deviceId = DeviceUtils.getDeviceId(context);
            Log.i(TAG, "device id : " + deviceId);
            AIUISetting.setSystemInfo(AIUIConstant.KEY_SERIAL_NUM, deviceId);
            mAIUIAgent = AIUIAgent.createAgent(context, getAIUIParams(), mAIUIListener);
        }
        if (null == mAIUIAgent) {
            Log.i(TAG, "创建AIUIAgent失败");

        } else {
            Log.i(TAG, "AIUIAgent已创建");
        }
    }

    public boolean IsCreatedAIUIAgent() {
        return mAIUIAgent != null;
    }

    public void DestroyAgent() {
        if (null != mAIUIAgent) {
            mAIUIAgent.destroy();
            mAIUIAgent = null;

            Log.i(TAG, "AIUIAgent已销毁");
        } else {
            Log.i(TAG, "AIUIAgent为空");
        }
    }

    public void StartRecord() {
        if (null == mAIUIAgent) {
            Log.i(TAG, "AIUIAgent为空，请先创建");
            Log.i(TAG, "AIUIAgent为空，请先创建");
            return;
        }

        Log.i(TAG, "开始语音识别");

        AIUIMessage wakeupMsg = new AIUIMessage(AIUIConstant.CMD_WAKEUP, 0, 0, "", null);
        mAIUIAgent.sendMessage(wakeupMsg);

        String params = "sample_rate=16000,data_type=audio,pers_param={\"uid\":\"\"},tag=audio-tag";
        AIUIMessage startRecord = new AIUIMessage(AIUIConstant.CMD_START_RECORD, 0, 0, params, null);

        mAIUIAgent.sendMessage(startRecord);
    }

    public void StopRecord() {
        if (null == mAIUIAgent) {
            Log.i(TAG, "AIUIAgent 为空，请先创建");
            return;
        }

        Log.i(TAG, "停止语音识别");

        String params = "sample_rate=16000,data_type=audio";
        AIUIMessage stopRecord = new AIUIMessage(AIUIConstant.CMD_STOP_RECORD, 0, 0, params, null);

        mAIUIAgent.sendMessage(stopRecord);
    }

    AIUIListener mAIUIListener = new AIUIListener() {

        @Override
        public void onEvent(AIUIEvent event) {
            Log.i(TAG, "on event: " + event.eventType);

            switch (event.eventType) {

                case AIUIConstant.EVENT_CONNECTED_TO_SERVER:
                    Log.i(TAG, "已连接服务器");
                    break;

                case AIUIConstant.EVENT_SERVER_DISCONNECTED:
                    Log.i(TAG, "与服务器断连");
                    break;

                case AIUIConstant.EVENT_WAKEUP:
                    Log.i(TAG, "进入识别状态");
                    eventInterface.OnWakeup();
                    break;
                case AIUIConstant.EVENT_SLEEP:
                    Log.i(TAG, "进入休眠状态");
                    eventInterface.OnSleep();
                    break;
                case AIUIConstant.EVENT_RESULT: {
                    try {
                        JSONObject bizParamJson = new JSONObject(event.info);
                        JSONObject data = bizParamJson.getJSONArray("data").getJSONObject(0);
                        JSONObject params = data.getJSONObject("params");
                        JSONObject content = data.getJSONArray("content").getJSONObject(0);

                        if (content.has("cnt_id")) {
                            String cnt_id = content.getString("cnt_id");
                            String cntStr = new String(event.data.getByteArray(cnt_id), "utf-8");

                            if (TextUtils.isEmpty(cntStr)) {
                                Log.i(TAG, "语音识别为空");
                                return;
                            }

                            JSONObject cntJson = new JSONObject(cntStr);
                            String sub = params.optString("sub");
                            if ("nlp".equals(sub)) {
                                String resultStr = cntJson.optString("intent");
                                Log.i(TAG, resultStr);
                                JSONObject resultJson = new JSONObject(resultStr);
                                String recordedText = resultJson.optString("text");
                                Log.i(TAG, "语音识别结果：" + recordedText);
                                if (eventInterface != null)
                                    eventInterface.OnRecordText(recordedText);
                                else Log.i(TAG, "eventInterface is null !");
                            }

                        }
                    } catch (Throwable e) {
                        e.printStackTrace();
                    }

                }
                break;

                case AIUIConstant.EVENT_ERROR: {
                    Log.i(TAG, "错误: " + event.arg1 + "\n" + event.info);
                }
                break;

                case AIUIConstant.EVENT_START_RECORD: {
                    Log.i(TAG, "已开始录音");
                    eventInterface.OnStartRecord();
                }
                break;

                case AIUIConstant.EVENT_STOP_RECORD: {
                    Log.i(TAG, "已停止录音");
                    eventInterface.OnStopRecord();
                }
                break;

                case AIUIConstant.EVENT_STATE: {
                    mAIUIState = event.arg1;

                    if (AIUIConstant.STATE_IDLE == mAIUIState) {
                        Log.i(TAG, "闲置状态，AIUI未开启");
                    } else if (AIUIConstant.STATE_READY == mAIUIState) {
                        Log.i(TAG, "AIUI已就绪，等待唤醒");
                    } else if (AIUIConstant.STATE_WORKING == mAIUIState) {
                        Log.i(TAG, "AIUI工作中，可进行交互");
                    }
                }
                break;

                default:
                    break;
            }
        }
    };

    String getAIUIParams() {
        String params = "";
        AssetManager assetManager = context.getResources().getAssets();

        try {
            InputStream ins = assetManager.open("cfg/aiui_phone.cfg");
            byte[] buffer = new byte[ins.available()];
            ins.read(buffer);
            ins.close();
            params = new String(buffer);
            JSONObject paramsJson = new JSONObject(params);
            params = paramsJson.toString();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (JSONException e) {
            e.printStackTrace();
        }

        return params;
    }

    public void SetEventCallBack(EventInterface eventInterface) {
        this.eventInterface = eventInterface;
    }
}