using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    public Display SortAlgorithm;
    public Display RunningTime;
    public Display ComparedCount;
    public Display SwappedCount;

    public TMP_Dropdown SelectSort;
    public Button StartShuffle;
    public Button StartSort;
    public Button ShuffleOnce;

    public Slider SpeedSlider;

    public Toggle Mute;
}
