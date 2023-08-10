using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;

public class KinectColorController : MonoBehaviour
{
    private Texture2D kinectCameraTexture;
    private Material kinectCameraMaterial;

    [SerializeField]
    private RawImage rawImageDisplay; 

    // Update is called once per frame
    void Update()
    {
        KinectManager.Instance.OpenKinect();
        var calibration = KinectManager.Instance.GetCalibration().ColorCameraCalibration;
        kinectCameraTexture = new Texture2D(calibration.ResolutionWidth, calibration.ResolutionHeight, TextureFormat.BGRA32, false);
        kinectCameraMaterial = GetComponent<Renderer>().material;

        using var capture = KinectManager.Instance.GetCapture();
        using var colorImage = capture.Color;

        var bgraArray = colorImage.GetPixels<BGRA>().ToArray();
        var colorData = new Color32[bgraArray.Length];

        for (int i = 0; i < colorData.Length; i++)
        {
            var index = colorData.Length - 1 - i;
            colorData[i] = new Color32(
                bgraArray[index].R, 
                bgraArray[index].G, 
                bgraArray[index].B, 
                bgraArray[index].A);
        }

        kinectCameraTexture.SetPixels32(colorData);
        kinectCameraTexture.Apply();

        kinectCameraMaterial.mainTexture = kinectCameraTexture;
        rawImageDisplay.texture = kinectCameraTexture;
    }
}
