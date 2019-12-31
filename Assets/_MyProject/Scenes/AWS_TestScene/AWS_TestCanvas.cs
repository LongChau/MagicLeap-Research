using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AWS_Test
{
    public class AWS_TestCanvas : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        [ContextMenu("GetApplicationPersistentDataPath")]
        private void GetApplicationPersistentDataPath()
        {
            Debug.Log($"{Application.persistentDataPath}");
        }

        public void GetListBucket()
        {
            AWS_Manager.Instance.GetListBucket();
        }

        public void Upload()
        {
            //byte[] fileData;
            //var filePath = $"{Application.persistentDataPath}/ScreenShot/Screen Shot 2019-12-17 at 16.17.19";

            //if (!File.Exists(filePath))
            //    return;

            //fileData = File.ReadAllBytes(filePath);

            //Debug.Log("Done upload");

            // "/Users/longchau/Library/Application Support/tongullman/testshooting/ScreenShot/ScreenShot2019_12_17.png"
            var fileName = "ScreenShot2019_12_17.png";
            var filePath = Path.Combine(Application.persistentDataPath, $"ScreenShot/{fileName}");
            AWS_Manager.Instance.PostObject(filePath, fileName);
        }
    }
}
