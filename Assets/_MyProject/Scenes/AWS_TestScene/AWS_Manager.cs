using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;

namespace AWS_Test
{
    public class AWS_Manager : MonoBehaviour
    {
        [Header("Amazon config:")]
        //arn:aws:s3:::magic-leap.test
        [SerializeField]
        private string _bucketName = "magic-leap.test";
        [SerializeField]
        private string _identityPoolId = "";
        [SerializeField]
        private string _cognitoIdentityRegion = RegionEndpoint.APSoutheast1.SystemName;
        [SerializeField]
        private string _s3Region = RegionEndpoint.APSoutheast1.SystemName;
        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        #region SIngleton
        private static AWS_Manager _instance;
        public static AWS_Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AWS_Manager>();
                    if (_instance == null)
                    {
                        GameObject newGO = new GameObject();
                        newGO.AddComponent<AWS_Manager>();
                    }
                }
                return _instance;
            }
        }
        #endregion


        public RegionEndpoint CognitoIdentityRegion => RegionEndpoint.GetBySystemName(_cognitoIdentityRegion);
        public RegionEndpoint S3Region => RegionEndpoint.GetBySystemName(_s3Region);

        private IAmazonS3 S3Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, S3Region);
                }
                //test comment
                return _s3Client;
            }
        }

        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(_identityPoolId, CognitoIdentityRegion);
                return _credentials;
            }
        }

        private void Awake()
        {
            _instance = this;

            // attach component to this object
            UnityInitializer.AttachToGameObject(this.gameObject);

            //LONG NOTE: Need to override this or it gonna be bug
            // this will force to use UnityWebRequest instead of obsoleted WWW
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }

        // Start is called before the first frame update
        void Start()
        {
            //LONG NOTE: Must have these lines to show log
            var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogTo = LoggingOptions.UnityLogger;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogResponsesSizeLimit = 4096;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
        }

        public void GetListBucket()
        {
            S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    responseObject.Response.Buckets.ForEach((s3b) =>
                    {
                        Debug.Log($"Bucket name: {s3b.BucketName}");
                    });
                }
                else
                {
                    Debug.Log($"AWS Error {responseObject.Exception}");
                }
            });
        }

        public void PostObject(string filePath, string fileName)
        {
            Debug.Log($"PostObject({fileName})");

            string resultText = "Retrieving the file";
            Debug.Log(resultText);

            var stream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read);

            resultText = "Creating request object";
            Debug.Log(resultText);

            var request = new PostObjectRequest
            {
                Bucket = _bucketName,
                Key = fileName,
                InputStream = stream,
                CannedACL = S3CannedACL.Private,

                // LONG note: AWS need this
                Region = S3Region
            };

            resultText = "Making HTTP post call";
            Debug.Log(resultText);

            S3Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    resultText = string.Format("object {0} posted to bucket {1}",
                        responseObj.Request.Key, responseObj.Request.Bucket);
                    Debug.Log(resultText);

                    resultText = "Done uploading file success";
                    Debug.Log(resultText);
                }
                else
                {
                    resultText = "Exception while posting the result object";
                    Debug.Log($"<color=red> {resultText} </color>");
                    resultText = $"receieved error {responseObj.Response.HttpStatusCode.ToString()}";
                    Debug.Log($"<color=red> {resultText} </color>");
                }
            });
        }
    }
}
