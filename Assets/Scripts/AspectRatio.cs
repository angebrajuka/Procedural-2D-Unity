using UnityEngine;
using System.Collections.Generic;
 
public class AspectRatio : MonoBehaviour {
 
    public static Vector2 screenSize;
    public static Vector3 halfScreen;
    private Camera cam;

    private void RescaleCamera() {
 
        if (Screen.width == screenSize.x && Screen.height == screenSize.y) return;
 
        float targetaspect = 16.0f / 9.0f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
 
        if (scaleheight < 1.0f) {
            Rect rect = cam.rect;
 
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
 
            cam.rect = rect;
        } else {
             // add pillarbox
            float scalewidth = 1.0f / scaleheight;
 
            Rect rect = cam.rect;
 
            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;
 
             cam.rect = rect;
        }
 
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;
        halfScreen = screenSize / 2;
    }
 
    void OnPreCull() {
        if (Application.isEditor) return;
        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);
 
        Camera.main.rect = nr;
        GL.Clear(true, true, Color.black);
     
        Camera.main.rect = wp;
 
    }
 
    void Start () {
        cam = GetComponent<Camera>();
        RescaleCamera();
    }
 
    void Update () {
        RescaleCamera();
    }
}