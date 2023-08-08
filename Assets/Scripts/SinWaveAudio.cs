using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWaveAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public float amplitude = 0.5f; // ����
    public float duration = 2f; // ��� �ð� (��)

    public int array_count = 1;

    [SerializeField] Dashboard dashboard;

    private void Start()
    {
        // AudioSource ������Ʈ�� �����ɴϴ�.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource ������Ʈ�� �����ϴ�.");
                return;
            }
        }
    }

    int max = 1000;
    int min = 200;

    public void PlaySinWave(float frequency)
    {
        int sampleRate = 44100;
        int numSamples = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[numSamples];

        for (int i = 0; i < numSamples; i++)
        {
            float time = i / (float)sampleRate;
            float value = amplitude * Mathf.Sin(2f * Mathf.PI * (min + (max/array_count)*frequency+1) * time);
            samples[i] = value;
        }

        audioSource.clip = AudioClip.Create("SinWave", numSamples, 1, sampleRate, false);
        audioSource.clip.SetData(samples, 0);
        audioSource.Play();
    }

    public void ToggleSound()
    {
        audioSource.mute = dashboard.Mute.isOn;
    }
}