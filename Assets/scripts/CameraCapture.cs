using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class CameraCapture : MonoBehaviour
{
    public Camera frontCamera;
    public RenderTexture renderTexture;

    void Start()
    {
        if (frontCamera == null)
            frontCamera = GetComponent<Camera>();

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(600, 400, 24);
            frontCamera.targetTexture = renderTexture;
        }
    }

    public Texture2D CaptureFrame()
    {
        // Capture the camera output as a Texture2D
        RenderTexture.active = renderTexture;
        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = null;
        return image;
    }

    public byte[] CaptureFrameAsPNG()
    {
        Texture2D image = CaptureFrame();
        return image.EncodeToPNG(); // Convert image to PNG format
    }
}
