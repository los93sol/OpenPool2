using UnityEngine;
using UnityEngine.UI;

public class KinectIrController : MonoBehaviour
{
    private Texture2D kinectCameraTexture;
    private Material kinectCameraMaterial;

    [SerializeField]
    private RawImage rawImageDisplay;

    // Update is called once per frame
    void Update()
    {
        KinectManager.Instance.OpenKinect();
        var calibration = KinectManager.Instance.GetCalibration().DepthCameraCalibration;
        kinectCameraTexture = new Texture2D(calibration.ResolutionWidth, calibration.ResolutionHeight, TextureFormat.BGRA32, false);
        kinectCameraMaterial = GetComponent<Renderer>().material;

        using var capture = KinectManager.Instance.GetCapture();
        using var irImage = capture.IR;

        var irArray = irImage.GetPixels<ushort>().ToArray();
        var colorData = new Color32[irArray.Length];

        for (int i = 0; i < colorData.Length; i++)
        {
            var index = colorData.Length - 1 - i;

            var minIr = 0;
            var maxIr = 750;
            var ir = irArray[i];
            byte irVal;

            if (ir < minIr)
            {
                irVal = 0;
            }
            if (ir > maxIr)
            {
                irVal = 255;
            }
            else
            {
                irVal = (byte)(((float)(ir - minIr) / (maxIr - minIr)) * 255);
            }

            //var depthVal = (byte)irArray[i];

            colorData[i] = new Color32(
                irVal,
                irVal,
                irVal,
                255);
        }

        kinectCameraTexture.SetPixels32(colorData);
        kinectCameraTexture.Apply();

        kinectCameraMaterial.mainTexture = kinectCameraTexture;
        rawImageDisplay.texture = kinectCameraTexture;
    }
}
