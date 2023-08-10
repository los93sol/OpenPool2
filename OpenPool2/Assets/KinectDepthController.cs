using OpenCvSharp;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class KinectDepthController : MonoBehaviour
{
    public RawImage rawImageDisplay;
    public Slider sliderMinDepth;
    public Slider sliderMaxDepth;

    private Texture2D kinectCameraTexture;
    private Material kinectCameraMaterial;    

    void Start()
    {
        sliderMinDepth.value = 0;
        sliderMaxDepth.value = 6_000;

        // TODO: Persist these, these are my values
        sliderMinDepth.value = 2110;
        sliderMaxDepth.value = 2335;
    }

    // Update is called once per frame
    void Update()
    {
        KinectManager.Instance.OpenKinect();
        //var calibration = KinectManager.Instance.GetCalibration().DepthCameraCalibration;
        //kinectCameraTexture = new Texture2D(calibration.ResolutionWidth, calibration.ResolutionHeight, TextureFormat.BGRA32, false);
        //kinectCameraMaterial = GetComponent<Renderer>().material;

        using var capture = KinectManager.Instance.GetCapture();
        using var depthImage = capture.Depth;

        var minDepth = sliderMinDepth.value;
        var maxDepth = sliderMaxDepth.value;

        //kinectCameraTexture = new Texture2D(depthImage.WidthPixels, depthImage.HeightPixels, TextureFormat.BGRA32, false);
        kinectCameraMaterial = GetComponent<Renderer>().material;

        // TEST: Try to get OpenCv working
        using var depthMat = new Mat(depthImage.HeightPixels, depthImage.WidthPixels, MatType.CV_16U, depthImage.Memory.ToArray());

        // Flip the image horizontally and vertically
        Cv2.Flip(depthMat, depthMat, FlipMode.XY);

        // Create a binary mask that is true for depth values within the desired range and false otherwise.
        using var tableMask = new Mat();
        Cv2.InRange(depthMat, new Scalar(minDepth), new Scalar(maxDepth), tableMask);

        // Invert the mask
        Cv2.Compare(tableMask, 0, tableMask, CmpType.EQ);

        // Use the inverse mask to set the depth values outside the desired range to 0
        depthMat.SetTo(0, tableMask);

        //Color32[] colorData = new Color32[depthMat.Rows * depthMat.Cols];

        //for (int y = 0; y < depthMat.Rows; y++)
        //{
        //    for (int x = 0; x < depthMat.Cols; x++)
        //    {
        //        ushort depthValue = depthMat.At<ushort>(y, x);

        //        // Normalize depthValue to 0-255 range
        //        byte depthVal = (byte)(((double)(depthValue - minDepth) / (maxDepth - minDepth)) * 255);

        //        // Assign the normalized value to the color array
        //        colorData[y * depthMat.Cols + x] = new Color32(depthVal, depthVal, depthVal, 255);
        //    }
        //}

        //kinectCameraTexture.SetPixels32(colorData);

        // Normalize the depth values to a float between 0 and 1.
        using var tableDepthFloat = new Mat();
        depthMat.ConvertTo(tableDepthFloat, MatType.CV_32F, 1.0 / (maxDepth - minDepth), -minDepth / (float)(maxDepth - minDepth));
        //depthMat.ConvertTo(tableDepthFloat, MatType.CV_16U, short.MaxValue / (maxDepth - minDepth), -minDepth * ushort.MaxValue / (maxDepth - minDepth));
        //depthMat.ConvertTo(tableDepthFloat, MatType.CV_8U, 255.0 / (maxDepth - minDepth), -minDepth * 255.0 / (maxDepth - minDepth));

        Color[] colorData = new Color[tableDepthFloat.Rows * tableDepthFloat.Cols];

        for (int y = 0; y < tableDepthFloat.Rows; y++)
        {
            for (int x = 0; x < tableDepthFloat.Cols; x++)
            {
                float depthValue = tableDepthFloat.At<float>(y, x);

                // Assign the float value to the color array
                colorData[(tableDepthFloat.Rows - 1 - y) * tableDepthFloat.Cols + x] = new Color(depthValue, depthValue, depthValue, 1f);
            }
        }

        kinectCameraTexture = new Texture2D(tableDepthFloat.Width, tableDepthFloat.Height, TextureFormat.BGRA32, false);
        kinectCameraTexture.SetPixels(colorData);

        //Color32[] colorData = new Color32[tableDepthFloat.Rows * tableDepthFloat.Cols];

        //for (int y = 0; y < tableDepthFloat.Rows; y++)
        //{
        //    for (int x = 0; x < tableDepthFloat.Cols; x++)
        //    {
        //        float depthValue = tableDepthFloat.At<float>(y, x);

        //        // Scale the float value (assuming it's between 0 and 1) to a byte.
        //        byte depthValByte = (byte)(depthValue);

        //        // Assign the scaled byte value to the color array
        //        colorData[(tableDepthFloat.Rows - 1 - y) * tableDepthFloat.Cols + x] = new Color32(depthValByte, depthValByte, depthValByte, 255);
        //    }
        //}

        //kinectCameraTexture.SetPixels32(colorData);

        //byte[] rawData = tableDepthFloat.ToBytes();
        //Color32[] colorData = new Color32[rawData.Length];

        //for (int i = 0; i < rawData.Length; i++)
        //{
        //    byte depthVal = rawData[i];
        //    colorData[i] = new Color32(depthVal, depthVal, depthVal, 255);
        //}

        //kinectCameraTexture.SetPixels32(colorData);

        //byte[] rawData = new byte[tableDepthFloat.Rows * tableDepthFloat.Cols];
        //for (int y = 0; y < tableDepthFloat.Rows; y++)
        //{
        //    for (int x = 0; x < tableDepthFloat.Cols; x++)
        //    {
        //        rawData[y * tableDepthFloat.Cols + x] = tableDepthFloat.At<byte>(y, x);
        //    }
        //}

        //kinectCameraTexture.LoadRawTextureData(rawData);

        //int byteCount = tableDepthFloat.Rows * tableDepthFloat.Cols * tableDepthFloat.ElemSize();
        //byte[] depthBytes = new byte[byteCount];

        //Marshal.Copy(tableDepthFloat.Data, depthBytes, 0, byteCount);

        ////float[] depthArray = new float[tableDepthFloat.Rows * tableDepthFloat.Cols * 4]; // Assuming CV_32FC4
        ////Marshal.Copy(tableDepthFloat.Data, depthArray, 0, depthArray.Length);
        ////kinectCameraTexture.LoadRawTextureData(depthArray);
        ////kinectCameraTexture.SetPixels(depthArray);

        //var depthBytes = depthMat.ToBytes();
        //kinectCameraTexture.LoadImage(depthBytes);
        //kinectCameraTexture.LoadRawTextureData(depthBytes);
        //// Convert CV_32F to CV_8UC4
        //using var depth4Channel = new Mat();
        //Cv2.CvtColor(tableDepthFloat, depth4Channel, ColorConversionCodes.GRAY2BGRA);

        //byte[] depthBytes = depth4Channel.ToBytes();

        //int width = depth4Channel.Width;
        //int height = depth4Channel.Height;

        //kinectCameraTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        //kinectCameraTexture.LoadRawTextureData(depthBytes);

        //// Convert to Texture2D
        //byte[] depthBytes = new byte[tableDepthFloat.Rows * tableDepthFloat.Cols];
        //Marshal.Copy(tableDepthFloat.Data, depthBytes, 0, depthBytes.Length);
        //kinectCameraTexture.LoadRawTextureData(depthBytes);

        //var depthArray = tableDepthFloat.ToBytes();
        //kinectCameraTexture.LoadImage(depthArray);
        //var depthArray = depthImage.GetPixels<ushort>().ToArray();
        //var colorData = new Color32[depthArray.Length];

        //for (int i = 0; i < colorData.Length; i++)
        //{
        //    var index = colorData.Length - 1 - i;

        //    var minDepth = sliderMinDepth.value;
        //    var maxDepth = sliderMaxDepth.value;

        //    var depth = depthArray[i];
        //    byte depthVal;

        //    if (depth < minDepth)
        //    {
        //        depthVal = 0;
        //    }
        //    if (depth > maxDepth)
        //    {
        //        depthVal = 255;
        //    }
        //    else
        //    {
        //        depthVal = (byte)(((float)(depth - minDepth) / (maxDepth - minDepth)) * 255);
        //    }

        //    colorData[i] = new Color32(
        //        depthVal,                
        //        depthVal,
        //        depthVal,
        //        255);
        //}

        //kinectCameraTexture.SetPixels32(colorData);
        kinectCameraTexture.Apply();

        kinectCameraMaterial.mainTexture = kinectCameraTexture;
        rawImageDisplay.texture = kinectCameraTexture;
    }
}
