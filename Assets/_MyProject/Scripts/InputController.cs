using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using TMPro;
using System.IO;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private GameObject _objPrefab;
    private MLInputController _controller;

    [SerializeField]
    private GameObject _bulletHold;

    [SerializeField]
    private AudioSource _shootSound;

    [SerializeField]
    private float _damage = 1f;

    [SerializeField]
    private TextMeshProUGUI _txtDebug;

    private string _debugText;

    // Start is called before the first frame update
    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += Hanle_OnButtonDown;
        _controller = MLInput.GetController(MLInput.Hand.Left);

        MLInput.OnControllerConnected += Handle_OnControllerConnected;
        MLInput.OnTriggerDown += Handle_OnTriggerDown;

        //MLCamera.OnCaptureCompleted += Handle_OnCaptureCompleted;
        CheckFilePathExist();
    }

    private void Handle_OnCaptureCompleted(MLCameraResultExtras cameraResult, string data)
    {
        Debug.Log($"Handle_OnCaptureCompleted({cameraResult}, {data})");

        _debugText = $"Handle_OnCaptureCompleted({cameraResult}, {data})";
        _txtDebug.SetText(_debugText);

        CheckFilePathExist();
    }

    private bool CheckFilePathExist(string path = "")
    {
        path = "Capture ML_20191122_11.43.45.jpg";
        if (File.Exists(path))
        {
            _debugText += $" Capture ML_20191122_11.43.45.jpg exists";
            _txtDebug.SetText(_debugText);
            return true;
        }

        _debugText += $" Capture ML_20191122_11.43.45.jpg is not existed";
        _txtDebug.SetText(_debugText);
        return false;
    }

    private void Handle_OnTriggerDown(byte controllerId, float triggerValue)
    {
        Debug.Log("Handle_OnTriggerDown");

        MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(triggerValue * 2.0f));
        _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, intensity);

        Shoot();
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(_controller.Position, transform.forward, out hit))
        {
            GameObject placeObj = Instantiate(_bulletHold, hit.point, Quaternion.Euler(hit.normal));
            _shootSound.Play();

            var target = hit.collider.gameObject.GetComponent<Target>();
            target?.Got(_damage);
        }
    }

    private void Handle_OnControllerConnected(byte controllerId)
    {
        Debug.Log("Handle_OnControllerConnected");
    }

    private void Hanle_OnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            Debug.Log("Hanle_OnButtonDown");
            RaycastHit hit;
            if (Physics.Raycast(_controller.Position, transform.forward, out hit))
            {
                GameObject placeObj = Instantiate(_objPrefab, hit.point, Quaternion.Euler(hit.normal));
            }
        }
    }

    private void OnDestroy()
    {
        MLInput.Stop();
        MLInput.OnControllerButtonDown -= Hanle_OnButtonDown;
        MLInput.OnTriggerDown -= Handle_OnTriggerDown;
    }
}
