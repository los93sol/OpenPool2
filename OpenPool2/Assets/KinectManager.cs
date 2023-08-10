using Microsoft.Azure.Kinect.Sensor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectManager : MonoBehaviour
{
    public static KinectManager Instance { get; private set; }

    private Device kinect;
    private bool camerasStarted;

    public GameObject ColorCamera;
    public GameObject IrCamera;
    public GameObject DepthCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        kinect?.Dispose();
    }

    public void OpenKinect()
    {
        // TODO: Semaphore to make this thread safe
        if (!camerasStarted)
        {
            // Open Kinect here
            kinect = Device.Open();
            kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R1080p,
                DepthMode = DepthMode.NFOV_Unbinned,
                SynchronizedImagesOnly = true
            });

            camerasStarted = true;
        }
    }

    public void CloseKinect()
    {
        // Close Kinect here
        kinect?.Dispose();
    }

    public Calibration GetCalibration() => kinect.GetCalibration();

    public Capture GetCapture() => kinect.GetCapture();

    public void ShowColorCamera()
    {        
        DepthCamera.SetActive(false);
        IrCamera.SetActive(false);

        ColorCamera.SetActive(true);
    }

    public void ShowIrCamera()
    {
        ColorCamera.SetActive(false);
        DepthCamera.SetActive(false);

        IrCamera.SetActive(true);
    }
    
    public void ShowDepthCamera()
    {
        ColorCamera.SetActive(false);
        IrCamera.SetActive(false);

        DepthCamera.SetActive(true);
    }
}
