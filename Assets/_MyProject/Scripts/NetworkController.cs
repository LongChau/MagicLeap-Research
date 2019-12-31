using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Amazon.S3.Model;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;
using Amazon.CognitoIdentity;
using System.IO;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour
{
    [Header("Amazon config:")]
    //arn:aws:s3:::magic-leap.test
    [SerializeField]
    private string bucketName = "magic-leap.test";
    [SerializeField]
    private string IdentityPoolId = "ap-southeast-1:f6a85fd1-221d-4f17-9f0c-990b569be76b";
    [SerializeField]
    private string CognitoIdentityRegion = RegionEndpoint.APSoutheast1.SystemName;
    [SerializeField]
    private string S3Region = RegionEndpoint.APSoutheast1.SystemName;

    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            //test comment
            return _s3Client;
        }
    }

    private static NetworkController _instance;
    public static NetworkController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkController>();
                if (_instance == null)
                {
                    GameObject newGO = new GameObject();
                    newGO.AddComponent<NetworkController>();
                }
            }
            return _instance;
        }
    }

    string resultText = "";
    public string ResultText
    {
        get => resultText;
        set
        {
            resultText = value;
            _txtStatus.text = ResultText;
        }
    }

    [SerializeField]
    private Text _txtStatus;

    private void Awake()
    {
        Debug.Log("Init AWS network manager");
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);

        //// Need to override this or it gonna be bug
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    void Start()
    {
        var loggingConfig = AWSConfigs.LoggingConfig;
        loggingConfig.LogTo = LoggingOptions.UnityLogger;
        loggingConfig.LogMetrics = true;
        loggingConfig.LogResponses = ResponseLoggingOption.Always;
        loggingConfig.LogResponsesSizeLimit = 4096;
        loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;

        // Need to override this or it gonna be bug
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    [ContextMenu("Test_UploadFile")]
    private void Test_UploadFile()
    {
        // /Users/longchau/Library/Application Support/tongullman/testshooting/ScreenShot/SavedScreen_54_25_25.jpeg
        PostObject(Path.Combine(Application.persistentDataPath, "ScreenShot/SavedScreen_54_25_25.jpeg"), "SavedScreen_54_25_25.jpeg");
    }

    public void PostObject(string filePath, string fileName)
    {
        Debug.Log($"PostObject({fileName})");
        Debug.Log("----- NetworkController::PostObject() -----" + filePath);

        ResultText = "Retrieving the file";
        Debug.Log(ResultText);

        var stream = new FileStream(filePath,
            FileMode.Open, FileAccess.Read, FileShare.Read);

        ResultText = "Creating request object";
        Debug.Log(ResultText);

        if (stream == null)
        {
            Debug.LogError("FileStream is null");
        }

        var request = new PostObjectRequest
        {
            Bucket = bucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,

            // LONG note: AWS need this
            Region = _S3Region
        };

        ResultText = "Making HTTP post call";
        Debug.Log(ResultText);

        Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                ResultText = string.Format("object {0} posted to bucket {1}",
                    responseObj.Request.Key, responseObj.Request.Bucket);
                Debug.Log(ResultText);

                ResultText = "Done uploading file success";
                Debug.Log(ResultText);
            }
            else
            {
                ResultText = "Exception while posting the result object";
                Debug.Log($"<color=red> {ResultText} </color>");
                ResultText = $"receieved error {responseObj.Response.HttpStatusCode.ToString()}";
                Debug.Log($"<color=red> {ResultText} </color>");
            }
        });
    }

    [ContextMenu("GetListBucket")]
    public void GetListBucket()
    {
        Debug.Log("----- NetworkController::GetListBucket() -----");
        Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log($"Bucket name: {s3b.BucketName}");
                    ResultText = $"Bucket name: {s3b.BucketName}";
                });
            }
            else
            {
                Debug.Log($"AWS Error {responseObject.Exception}");
                ResultText = $"AWS Error {responseObject.Exception}";
            }
        });
    }

    public void Handle_OnCompletedCaptureImage(string filePath, string fileName)
    {
        Debug.Log("----- NetworkController::Handle_OnCompletedCaptureImage() -----");
        PostObject(filePath, fileName);
    }
}

//// %BANNER_BEGIN%
//// ---------------------------------------------------------------------
//// %COPYRIGHT_BEGIN%
////
//// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
//// Use of this file is governed by the Creator Agreement, located
//// here: https://id.magicleap.com/creator-terms
////
//// %COPYRIGHT_END%
//// ---------------------------------------------------------------------
//// %BANNER_END%

//using System;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.XR.MagicLeap;
//using System.Collections.Generic;
//using System.Threading;
//using System.IO;
//using Amazon.S3.Model;
//using Amazon.S3;
//using Amazon.Runtime;
//using Amazon;
//using Amazon.CognitoIdentity;

//namespace MagicLeap
//{
//    [RequireComponent(typeof(PrivilegeRequester))]
//    public class ImageCaptureExample : MonoBehaviour
//    {
//        [Serializable]
//        private class ImageCaptureEvent : UnityEvent<Texture2D> { }
//        [Serializable]
//        private class ImageCaptureCompletedEvent : UnityEvent<string, string> { }

//        #region Private Variables
//        [SerializeField, Space, Tooltip("ControllerConnectionHandler reference.")]
//        private ControllerConnectionHandler _controllerConnectionHandler = null;

//        [SerializeField, Space]
//        private ImageCaptureEvent OnImageReceivedEvent = null;
//        [SerializeField, Space]
//        private ImageCaptureCompletedEvent OnImageCaptureCompletedEvent = null;

//        private bool _isCameraConnected = false;
//        private bool _isCapturing = false;
//        private bool _hasStarted = false;
//        private bool _doPrivPopup = false;
//        private bool _hasShownPrivPopup = false;
//        private Thread _captureThread = null;

//        /// <summary>
//        /// The example is using threads on the call to MLCamera.CaptureRawImageAsync to alleviate the blocking
//        /// call at the beginning of CaptureRawImageAsync, and the safest way to prevent race conditions here is to
//        /// lock our access into the MLCamera class, so that we don't accidentally shut down the camera
//        /// while the thread is attempting to work
//        /// </summary>
//        private object _cameraLockObject = new object();

//        private PrivilegeRequester _privilegeRequester = null;

//        private string filePath = "";
//        private string fileName = "";
//        #endregion

//        #region Unity Methods

//        /// <summary>
//        /// Using Awake so that Privileges is set before PrivilegeRequester Start.
//        /// </summary>
//        void Awake()
//        {
//            if (_controllerConnectionHandler == null)
//            {
//                Debug.LogError("Error: ImageCaptureExample._controllerConnectionHandler is not set, disabling script.");
//                enabled = false;
//                return;
//            }

//            // If not listed here, the PrivilegeRequester assumes the request for
//            // the privileges needed, CameraCapture in this case, are in the editor.
//            _privilegeRequester = GetComponent<PrivilegeRequester>();

//            // Before enabling the Camera, the scene must wait until the privilege has been granted.
//            _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
//        }

//        [ContextMenu("TestWriteFile")]
//        private void TestWriteFile()
//        {
//            // Create a texture the size of the screen, RGB24 format
//            int width = Screen.width;
//            int height = Screen.height;
//            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

//            // Read screen contents into the texture
//            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
//            tex.Apply();

//            // Encode texture into PNG
//            byte[] bytes = tex.EncodeToJPG();
//            Destroy(tex);

//            string path = Path.Combine(Application.persistentDataPath, "ScreenShot");
//            Debug.Log($"<color=blue> {path} </color>");

//            if (!File.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            // save that file
//            fileName = $"SavedScreen_{(int)Time.time}_{DateTime.Now.Second}_{DateTime.Now.Day}.jpeg";
//            filePath = Path.Combine(path, fileName);
//            File.WriteAllBytes(filePath, bytes);

//            // Send event done
//            //OnImageCaptureCompletedEvent?.Invoke(filePath, fileName);

//            NetworkController.Instance.PostObject(filePath, fileName);
//        }

//        /// <summary>
//        /// Stop the camera, unregister callbacks, and stop input and privileges APIs.
//        /// </summary>
//        void OnDisable()
//        {
//            MLInput.OnControllerButtonDown -= OnButtonDown;
//            lock (_cameraLockObject)
//            {
//                if (_isCameraConnected)
//                {
//                    MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;
//                    _isCapturing = false;
//                    DisableMLCamera();
//                }
//            }
//        }

//        /// <summary>
//        /// Cannot make the assumption that a reality privilege is still granted after
//        /// returning from pause. Return the application to the state where it
//        /// requests privileges needed and clear out the list of already granted
//        /// privileges. Also, disable the camera and unregister callbacks.
//        /// </summary>
//        void OnApplicationPause(bool pause)
//        {
//            if (pause)
//            {
//                lock (_cameraLockObject)
//                {
//                    if (_isCameraConnected)
//                    {
//                        MLCamera.OnRawImageAvailable -= OnCaptureRawImageComplete;
//                        _isCapturing = false;
//                        DisableMLCamera();
//                    }
//                }

//                MLInput.OnControllerButtonDown -= OnButtonDown;

//                _hasStarted = false;
//            }
//        }

//        void OnDestroy()
//        {
//            if (_privilegeRequester != null)
//            {
//                _privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
//            }
//        }

//        private void Update()
//        {
//            if (_doPrivPopup && !_hasShownPrivPopup)
//            {
//                Instantiate(Resources.Load("PrivilegeDeniedError"));
//                _doPrivPopup = false;
//                _hasShownPrivPopup = true;
//            }
//        }
//        #endregion

//        #region Public Methods
//        /// <summary>
//        /// Captures a still image using the device's camera and returns
//        /// the data path where it is saved.
//        /// </summary>
//        /// <param name="fileName">The name of the file to be saved to.</param>
//        public void TriggerAsyncCapture()
//        {
//            if (_captureThread == null || (!_captureThread.IsAlive))
//            {
//                ThreadStart captureThreadStart = new ThreadStart(CaptureThreadWorker);
//                _captureThread = new Thread(captureThreadStart);
//                _captureThread.Start();
//            }
//            else
//            {
//                Debug.Log("Previous thread has not finished, unable to begin a new capture just yet.");
//            }
//        }
//        #endregion

//        #region Private Functions
//        /// <summary>
//        /// Connects the MLCamera component and instantiates a new instance
//        /// if it was never created.
//        /// </summary>
//        private void EnableMLCamera()
//        {
//            lock (_cameraLockObject)
//            {
//                MLResult result = MLCamera.Start();
//                if (result.IsOk)
//                {
//                    result = MLCamera.Connect();
//                    _isCameraConnected = true;
//                }
//                else
//                {
//                    if (result.Code == MLResultCode.PrivilegeDenied)
//                    {
//                        Instantiate(Resources.Load("PrivilegeDeniedError"));
//                    }

//                    Debug.LogErrorFormat("Error: ImageCaptureExample failed starting MLCamera, disabling script. Reason: {0}", result);
//                    enabled = false;
//                    return;
//                }
//            }
//        }

//        /// <summary>
//        /// Disconnects the MLCamera if it was ever created or connected.
//        /// </summary>
//        private void DisableMLCamera()
//        {
//            lock (_cameraLockObject)
//            {
//                if (MLCamera.IsStarted)
//                {
//                    MLCamera.Disconnect();
//                    // Explicitly set to false here as the disconnect was attempted.
//                    _isCameraConnected = false;
//                    MLCamera.Stop();
//                }
//            }
//        }

//        /// <summary>
//        /// Once privileges have been granted, enable the camera and callbacks.
//        /// </summary>
//        private void StartCapture()
//        {
//            if (!_hasStarted)
//            {
//                lock (_cameraLockObject)
//                {
//                    EnableMLCamera();
//                    MLCamera.OnRawImageAvailable += OnCaptureRawImageComplete;
//                }
//                MLInput.OnControllerButtonDown += OnButtonDown;

//                _hasStarted = true;
//            }
//        }
//        #endregion

//        #region Event Handlers
//        /// <summary>
//        /// Responds to privilege requester result.
//        /// </summary>
//        /// <param name="result"/>
//        private void HandlePrivilegesDone(MLResult result)
//        {
//            if (!result.IsOk)
//            {
//                if (result.Code == MLResultCode.PrivilegeDenied)
//                {
//                    Instantiate(Resources.Load("PrivilegeDeniedError"));
//                }

//                Debug.LogErrorFormat("Error: ImageCaptureExample failed to get requested privileges, disabling script. Reason: {0}", result);
//                enabled = false;
//                return;
//            }

//            Debug.Log("Succeeded in requesting all privileges");
//            StartCapture();
//        }

//        /// <summary>
//        /// Handles the event for button down.
//        /// </summary>
//        /// <param name="controllerId">The id of the controller.</param>
//        /// <param name="button">The button that is being pressed.</param>
//        private void OnButtonDown(byte controllerId, MLInputControllerButton button)
//        {
//            if (_controllerConnectionHandler.IsControllerValid(controllerId) && MLInputControllerButton.Bumper == button && !_isCapturing)
//            {
//                TriggerAsyncCapture();
//            }
//        }

//        /// <summary>
//        /// Handles the event of a new image getting captured.
//        /// </summary>
//        /// <param name="imageData">The raw data of the image.</param>
//        private void OnCaptureRawImageComplete(byte[] imageData)
//        {
//            lock (_cameraLockObject)
//            {
//                _isCapturing = false;
//            }
//            // Initialize to 8x8 texture so there is no discrepency
//            // between uninitalized captures and error texture
//            Texture2D texture = new Texture2D(8, 8);
//            bool status = texture.LoadImage(imageData);

//            if (status && (texture.width != 8 && texture.height != 8))
//            {
//                OnImageReceivedEvent.Invoke(texture);
//            }

//            //--- Save to persistent path ---
//            // save that file
//            string path = Path.Combine(Application.persistentDataPath, "ScreenShot");
//            Debug.Log($"<color=blue> {path} </color>");

//            if (!File.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//            }

//            // save that file
//            fileName = $"SavedScreen_{(int)Time.time}_{DateTime.Now.Second}_{DateTime.Now.Day}.jpeg";
//            filePath = Path.Combine(path, fileName);
//            File.WriteAllBytes(filePath, imageData);

//            // send event
//            //OnImageCaptureCompletedEvent?.Invoke(filePath, fileName);

//            Invoke("Test", 3f);
//            //NetworkController.Instance.GetListBucket();
//            //NetworkController.Instance.PostObject(filePath, fileName);
//        }

//        private void Test()
//        {
//            //NetworkController.Instance.GetListBucket();
//            //NetworkController.Instance.ReadFile(filePath, fileName);
//            NetworkController.Instance.PostObject(filePath, fileName);
//        }

//        /// <summary>
//        /// Worker function to call the API's Capture function
//        /// </summary>
//        private void CaptureThreadWorker()
//        {
//            lock (_cameraLockObject)
//            {
//                if (MLCamera.IsStarted && _isCameraConnected)
//                {
//                    MLResult result = MLCamera.CaptureRawImageAsync();
//                    if (result.IsOk)
//                    {
//                        _isCapturing = true;
//                    }
//                    else if (result.Code == MLResultCode.PrivilegeDenied)
//                    {
//                        _doPrivPopup = true;
//                    }
//                }
//            }
//        }
//        #endregion
//    }
//}

