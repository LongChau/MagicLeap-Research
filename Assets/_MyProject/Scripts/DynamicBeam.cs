using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(LineRenderer))]
public class DynamicBeam : MonoBehaviour
{
    [SerializeField]
    private GameObject _controller;
    [SerializeField]
    private LineRenderer _beamLine;

    [SerializeField]
    public Color _startColor;
    [SerializeField]
    public Color _endColor;

    //private void OnValidate()
    //{
    //    _beamLine = GetComponent<LineRenderer>();
    //}

    // Start is called before the first frame update
    void Start()
    {
        _beamLine.startColor = _startColor;
        _beamLine.endColor = _endColor;
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        transform.position = _controller.transform.position;
        transform.rotation = _controller.transform.rotation;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            _beamLine.useWorldSpace = true;
            _beamLine.SetPosition(0, transform.position);
            _beamLine.SetPosition(1, hit.point);
        }
        else
        {
            _beamLine.useWorldSpace = false;
            _beamLine.SetPosition(0, transform.position);
            _beamLine.SetPosition(1, Vector3.forward * 5);
        }
    }
}
