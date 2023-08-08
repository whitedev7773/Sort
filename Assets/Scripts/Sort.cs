using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class Sort : MonoBehaviour
{
    [SerializeField] GameObject target_clone;
    [SerializeField] int target_clone_count = 0;
    [SerializeField] float interval = 0f;
    [SerializeField] float scale = 1f;
    [SerializeField] float y_offset = 0f;
    public float delay = 0.005f;
    public List<Transform> array = new List<Transform>();

    private IEnumerator running_shuffle_coroutine;

    public Dashboard dashboard;
    private Timer timer;

    public Color default_color;
    public Color selected_color;
    public Color finding_color;

    int p = 0;  // For Quick Sort

    public SinWaveAudio Sound;

    private int compared_count = 0;
    private int swapped_count = 0;

    void Start()
    {
        if (target_clone_count % 2 == 1)
        {
            Debug.LogError("클론 카운트는 짝수여야함");
            return;
        }

        running_shuffle_coroutine = _Shuffle();

        timer = GetComponent<Timer>();

        dashboard.SelectSort.interactable = false;
        dashboard.StartShuffle.interactable = false;
        dashboard.StartSort.interactable = false;
        dashboard.ShuffleOnce.interactable = false;

        Sound.array_count = target_clone_count;
        p = (int)(target_clone_count / 2);

        // 클론 시작
        StartCoroutine(Clone());
    }

    private IEnumerator Clone()
    {
        float clone_start = (-target_clone_count / 2) - (interval * (target_clone_count / 2));

        Vector3 start_scale = new Vector3(1, 1 * scale, 1);
        target_clone.transform.localScale = start_scale;

        Vector3 start_pos = new Vector3(clone_start, start_scale.y/2 + y_offset, 0);
        target_clone.transform.position = start_pos;

        array.Add(target_clone.transform);
        Sound.PlaySinWave(target_clone.transform.localScale.y);

        yield return new WaitForSeconds(delay);

        for (int i = 1; i < target_clone_count; i++)
        {
            GameObject clone = Instantiate(target_clone);

            Vector3 clone_scale = new Vector3(1, (i + 1)*scale, 1);
            clone.transform.localScale = clone_scale;

            Vector3 clone_pos = new Vector3(i + clone_start + interval * i, clone_scale.y/2 + y_offset, 0);
            clone.transform.position = clone_pos;

            array.Add(clone.transform);
            Sound.PlaySinWave(clone.transform.localScale.y);

            yield return new WaitForSeconds(delay);
        }

        dashboard.SelectSort.interactable = true;
        dashboard.StartShuffle.interactable = true;
        dashboard.StartSort.interactable = true;
        dashboard.ShuffleOnce.interactable = true;
    }

    private bool while_shuffle = false;
    public void ToggleShuffle()
    {
        if (while_shuffle)
        {
            StopCoroutine(running_shuffle_coroutine);
            timer.StopTimer();
            dashboard.StartShuffle.GetComponentInChildren<TextMeshProUGUI>().text = "서플 시작";
            dashboard.SortAlgorithm.ChangeText("대기 중");
            while_shuffle = false;
            dashboard.SelectSort.interactable = true;
            dashboard.StartSort.interactable = true;
            dashboard.ShuffleOnce.interactable = true;
            running_shuffle_coroutine = _Shuffle();  // 새로 갱신
        }
        else
        {
            StartCoroutine(running_shuffle_coroutine);
            timer.StartTimer();
            dashboard.StartShuffle.GetComponentInChildren<TextMeshProUGUI>().text = "서플 중단";
            dashboard.SortAlgorithm.ChangeText("섞는 중");
            while_shuffle = true;
            dashboard.SelectSort.interactable = false;
            dashboard.StartSort.interactable = false;
            dashboard.ShuffleOnce.interactable = false;
        }
    }

    bool shuffle_once = false;
    public void ShuffleOnce()
    {
        running_shuffle_coroutine = _Shuffle();  // 새로 갱신
        shuffle_once = true;
        while_shuffle = true;
        StartCoroutine(running_shuffle_coroutine);
        timer.StartTimer();
        dashboard.StartShuffle.GetComponentInChildren<TextMeshProUGUI>().text = "서플 중단";
        dashboard.SortAlgorithm.ChangeText("섞는중");
        dashboard.SelectSort.interactable = false;
        dashboard.StartSort.interactable = false;
        dashboard.ShuffleOnce.interactable = false;
    }

    private IEnumerator _Shuffle()
    {
        timer.ResetTimer();
        while (true)
        {
            for (int i = 0; i < array.Count; i++)
            {
                int A_index = i;
                int B_index = UnityEngine.Random.Range(0, array.Count);

                // Sound.PlaySinWave(array[B_index].transform.localScale.y);

                array[A_index].GetComponent<SpriteRenderer>().color = selected_color;
                array[B_index].GetComponent<SpriteRenderer>().color = selected_color;

                Sound.PlaySinWave(B_index);
                Swap(A_index, B_index);
                yield return new WaitForSeconds(delay);

                array[A_index].GetComponent<SpriteRenderer>().color = default_color;
                array[B_index].GetComponent<SpriteRenderer>().color = default_color;
            }
            if (shuffle_once)
            {
                while_shuffle = false;
                StopCoroutine(running_shuffle_coroutine);
                timer.StopTimer();
                dashboard.StartShuffle.GetComponentInChildren<TextMeshProUGUI>().text = "서플 시작";
                dashboard.SortAlgorithm.ChangeText("대기중");
                dashboard.SelectSort.interactable = true;
                dashboard.StartSort.interactable = true;
                dashboard.ShuffleOnce.interactable = true;
                shuffle_once = false;
                running_shuffle_coroutine = _Shuffle();  // 새로 갱신
                yield break;
            }
        }
    }

    private void Swap(int A_index, int B_index)
    {
        Transform A = array[A_index];
        float _A_y_scale = array[A_index].localScale.y;
        Transform B = array[B_index];

        SetScale(A, B.localScale.y);
        SetScale(B, _A_y_scale);

        // Sound.PlaySinWave(B.localScale.y);
        // Sound.PlaySinWave(_A_y_scale);
    }

    private void SetScale(Transform target, float y_size)
    {
        target.localScale = new Vector3(1, y_size, 1);
        target.position = new Vector3(target.position.x, (y_size / 2) + y_offset, 0);
    }

    public void StartSort()
    {
        IEnumerator[] sorts =
        {
            BubbleSort(),
            SelectionSort(),
            InsertionSort(),
            QuickSort(0, array.Count - 1)
        };

        timer.ResetTimer();
        timer.StartTimer();
        dashboard.SelectSort.interactable = false;
        dashboard.StartShuffle.interactable = false;
        dashboard.ShuffleOnce.interactable = false;
        dashboard.StartSort.interactable = false;

        StartCoroutine(_StartSort(sorts[dashboard.SelectSort.value]));
        
    }

    public IEnumerator _StartSort(IEnumerator sort_type)
    {
        InitCount();
        yield return StartCoroutine(sort_type);
        yield return StartCoroutine(EndSort());
    }

    public IEnumerator BubbleSort()
    {
        dashboard.SortAlgorithm.ChangeText("버블");
        int n = array.Count;
        for (int i=0; i<n-1; i++)
        {
            for (int j=0; j<n-i-1; j++)
            {
                array[j].GetComponent<SpriteRenderer>().color = selected_color;
                array[j+1].GetComponent<SpriteRenderer>().color = finding_color;

                Sound.PlaySinWave(j);
                // yield return new WaitForSeconds(delay);

                SetComparedCount(compared_count + 1);
                if (array[j].localScale.y > array[j+1].localScale.y)
                {
                    Swap(j, j + 1);
                    SetSwappedCount(swapped_count + 1);
                    yield return new WaitForSeconds(delay);
                }

                array[j].GetComponent<SpriteRenderer>().color = default_color;
                array[j + 1].GetComponent<SpriteRenderer>().color = default_color;
            }
        }
    }

    public IEnumerator SelectionSort()
    {
        dashboard.SortAlgorithm.ChangeText("선택");

        int n = array.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;

            // 최솟값을 찾습니다.
            for (int j = i + 1; j < n; j++)
            {
                array[j].GetComponent<SpriteRenderer>().color = finding_color;

                SetComparedCount(compared_count + 1);
                Sound.PlaySinWave(j);
                yield return new WaitForSeconds(delay);

                array[j].GetComponent<SpriteRenderer>().color = default_color;

                if (array[j].localScale.y < array[minIndex].localScale.y)
                {
                    minIndex = j;
                }
            }
            
            array[minIndex].GetComponent<SpriteRenderer>().color = selected_color;
            array[i].GetComponent<SpriteRenderer>().color = finding_color;

            // Sound.PlaySinWave(array[i].transform.localScale.y);
            Swap(minIndex, i);
            SetSwappedCount(swapped_count + 1);
            yield return new WaitForSeconds(delay);

            array[minIndex].GetComponent<SpriteRenderer>().color = default_color;
            array[i].GetComponent<SpriteRenderer>().color = default_color;
        }
    }

    public IEnumerator InsertionSort()
    {
        dashboard.SortAlgorithm.ChangeText("삽입");

        int n = array.Count;

        for (int k = 1; k < n; k++)
        {
            int key = k;
            int previous = key - 1;

            // key보다 큰 원소들을 오른쪽으로 이동시키며 key를 적절한 위치에 삽입합니다.
            while (previous >= 0 && array[previous].localScale.y > array[key].localScale.y)
            {
                SetComparedCount(compared_count + 1);
                Sound.PlaySinWave(key);
                // yield return new WaitForSeconds(delay/3);
                array[key].GetComponent<SpriteRenderer>().color = selected_color;
                array[previous].GetComponent<SpriteRenderer>().color = finding_color;

                Swap(key, previous);
                SetSwappedCount(swapped_count + 1);
                yield return new WaitForSeconds(delay);

                array[key].GetComponent<SpriteRenderer>().color = default_color;
                array[previous].GetComponent<SpriteRenderer>().color = default_color;

                key--;
                previous--;
            }

            // yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator QuickSort(int low, int high)
    {
        if (low < high)
        {
            yield return StartCoroutine(Partition(low, high));
            yield return array[p].GetComponent<SpriteRenderer>().color = finding_color;
            yield return StartCoroutine(QuickSort(low, p - 1));
            yield return StartCoroutine(QuickSort(p + 1, high));
        }
        else
        {
            array[p].GetComponent<SpriteRenderer>().color = default_color;
        }
    }

    IEnumerator Partition(int low, int high)
    {
        Transform pivot = array[high];
        int i = low - 1;

        array[low].GetComponent<SpriteRenderer>().color = selected_color;

        for (int j = low; j < high; j++)
        {
            SetComparedCount(compared_count + 1);
            Sound.PlaySinWave(j);
            array[j].GetComponent<SpriteRenderer>().color = selected_color;
            if (array[j].localScale.y < pivot.localScale.y)
            {
                i++;
                Swap(i, j);
                SetSwappedCount(swapped_count + 1);
                yield return new WaitForSeconds(delay);
            }
            array[j].GetComponent<SpriteRenderer>().color = default_color;
        }

        Sound.PlaySinWave(i);
        Swap(i + 1, high);
        SetSwappedCount(swapped_count + 1);
        p = i + 1;
        yield return new WaitForSeconds(delay);
        array[low].GetComponent<SpriteRenderer>().color = default_color;
    }

    public IEnumerator EndSort()
    {
        timer.StopTimer();

        for (int j=0; j<array.Count; j++)
        {
            array[j].GetComponent<SpriteRenderer>().color = selected_color;
            Sound.PlaySinWave(array[j].transform.localScale.y);

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1);
        for (int j = 0; j < array.Count; j++)
        {
            array[j].GetComponent<SpriteRenderer>().color = default_color;
        }

        dashboard.SortAlgorithm.ChangeText("대기중");
        dashboard.SelectSort.interactable = true;
        dashboard.StartShuffle.interactable = true;
        dashboard.ShuffleOnce.interactable = true;
        dashboard.StartSort.interactable = true;
    }

    void Update()
    {
        Time.timeScale = dashboard.SpeedSlider.value;
    }

    private void InitCount()
    {
        SetComparedCount(0);
        SetSwappedCount(0);
    }

    private void SetComparedCount(int value)
    {
        compared_count = value;
        dashboard.ComparedCount.ChangeText($"{compared_count}회");
    }

    private void SetSwappedCount(int value)
    {
        swapped_count = value;
        dashboard.SwappedCount.ChangeText($"{swapped_count}회");
    }
}