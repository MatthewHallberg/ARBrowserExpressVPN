package com.example.matthew.webViewPlugin;
import com.unity3d.player.UnityPlayer;

import android.graphics.Canvas;
import android.util.Log;
import android.webkit.WebSettings;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebChromeClient;
import android.webkit.WebViewClient;

public class WebBridge {

    BitmapWebView bitmapWebView;
    PluginCallback UnityBitmapCallback;
    Canvas tempCanvas;


    public void init(){
        bitmapWebView = new BitmapWebView(UnityPlayer.currentActivity);
        tempCanvas = new Canvas();
        bitmapWebView.setDrawingCacheEnabled(true);
        bitmapWebView.setVisibility(View.VISIBLE);
        bitmapWebView.setAlpha(0.0f);

        WebSettings webSettings = bitmapWebView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setUseWideViewPort(true);
        webSettings.setDefaultTextEncodingName("utf-8");
        webSettings.setDomStorageEnabled(true);

        bitmapWebView.measure(100, 100);

        bitmapWebView.enableSlowWholeDocumentDraw();

        bitmapWebView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageFinished(WebView view, String url) {
                if (view.getVisibility() != View.VISIBLE) {
                    view.setVisibility(View.VISIBLE);
                }
                view.draw(tempCanvas);
                view.refreshDrawableState();
                UnityBitmapCallback.onFrameUpdate(bitmapWebView.readData);
            }
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                view.loadUrl(url);
                return true;
            }
            });


        bitmapWebView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int progress) {
                Log.e("blah","progress" + bitmapWebView.getProgress());
            }
        });
    }

    public void SetUnityBitmapCallback(PluginCallback callback){
        Log.e("AndroidUnity","from android: my unity callback is set");
        UnityBitmapCallback = callback;
    }

    public void GetBitmap(String unityURL){
        bitmapWebView.loadUrl(unityURL);
        Log.e("blah","loading URL: ");
    }
}
