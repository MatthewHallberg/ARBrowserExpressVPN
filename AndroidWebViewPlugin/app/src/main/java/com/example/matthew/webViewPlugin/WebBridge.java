package com.example.matthew.webViewPlugin;

import android.util.Log;
import com.unity3d.player.UnityPlayer;

public class WebBridge {
    PluginCallback UnityBitmapCallback;
    ImageHelper imageHelper;

    public void init(){
        imageHelper = new ImageHelper(UnityPlayer.currentActivity,this);
    }

    public void SetUnityBitmapCallback(PluginCallback callback){
        Log.e("AndroidUnity","from android: my unity callback is set");
        UnityBitmapCallback = callback;
    }

    public void GetBitmap(final String unityURL){
        imageHelper.loadWeb(unityURL);
        Log.e("blah","loading URL: " + unityURL);
    }
}
