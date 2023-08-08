using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] Dashboard dashboard;

    private float elapsedTime;  // ��� �ð�
    private bool isRunning;  // �����ġ�� ���� ������ ����

    void Start()
    {
        elapsedTime = 0f;
        isRunning = false;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        StartCoroutine(UpdateStopwatch());
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        dashboard.RunningTime.ChangeText(elapsedTime.ToString("F4") + "��");
    }

    private IEnumerator UpdateStopwatch()
    {
        while (isRunning)
        {
            yield return null;
        }
    }
}
