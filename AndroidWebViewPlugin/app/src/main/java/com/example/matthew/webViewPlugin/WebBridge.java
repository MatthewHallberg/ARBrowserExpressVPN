package com.example.matthew.webViewPlugin;
import com.unity3d.player.UnityPlayer;
import android.graphics.Canvas;

public class WebBridge {


    BitmapWebView bitmapWebView;
    Canvas tempCanvas;


    public void init(){
        bitmapWebView = new BitmapWebView(UnityPlayer.currentActivity);
        tempCanvas = new Canvas();
    }

    public void GetBitmap(String unityURL){
        bitmapWebView.loadUrl(unityURL);
        bitmapWebView.draw(tempCanvas);
    }
}
