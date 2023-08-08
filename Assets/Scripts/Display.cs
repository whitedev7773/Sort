using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Display : MonoBehaviour
{
    private TextMeshProUGUI TMP;
    [SerializeField] private string ShowName, ShowText;

    private void Start()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        Apply();
    }

    public void ChangeName(string name)
    {
        ShowName = name;
        Apply();
    }

    public void ChangeText(string text)
    {
        ShowText = text;
        Apply();
    }

    private void Apply()
    {
        TMP.text = $"{ShowName} : {ShowText}";
    }
}
