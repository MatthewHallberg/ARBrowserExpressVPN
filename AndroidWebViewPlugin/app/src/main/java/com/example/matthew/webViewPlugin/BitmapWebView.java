package com.example.matthew.webViewPlugin;

import com.unity3d.player.UnityPlayer;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.content.Context;
import java.io.ByteArrayOutputStream;
import java.io.UnsupportedEncodingException;
import java.nio.charset.StandardCharsets;

import android.util.Log;
import android.webkit.WebView;

public class BitmapWebView extends WebView implements PluginCallback {

    ByteArrayOutputStream stream;
    Bitmap bm;
    Canvas bmCanvas;
    ReadData readData;

    public BitmapWebView(Context context) {
        super(context);
        stream = new ByteArrayOutputStream();
        bm = Bitmap.createBitmap(200, 300, Bitmap.Config.ARGB_8888);
        bmCanvas = new Canvas(bm);
        readData = new ReadData(new byte[]{});
    }

    public void onFrameUpdate(ReadData newData){
        readData.Buffer = newData.Buffer;
    }

    @Override
    public void draw(Canvas canvas) {
        // draw onto a new canvas
        super.draw(bmCanvas);
        bm.compress(Bitmap.CompressFormat.PNG, 100, stream);
        Log.e("yo", "SIZE FROM ANDROID: " + stream.toByteArray().length);
        readData.Buffer = stream.toByteArray();
        onFrameUpdate(readData);
        stream.reset();
    }
}
