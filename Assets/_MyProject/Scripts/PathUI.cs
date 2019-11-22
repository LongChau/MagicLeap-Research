using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtPath;

    // Start is called before the first frame update
    void Start()
    {
        _txtPath.SetText($"Path: {Application.persistentDataPath}");
    }
}
