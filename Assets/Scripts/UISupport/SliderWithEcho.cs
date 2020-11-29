using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithEcho : MonoBehaviour
{
    //Behavior from SliderWithEcho.cs example, Week 2
    public Slider slider = null;
    public Text echo = null;

    public delegate void SliderCallBackDelegate(float v);
    private SliderCallBackDelegate mCallBack = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(slider != null);
        Debug.Assert(echo != null);

        slider.onValueChanged.AddListener(SliderValueChange);
        SetValue(slider.value);
    }

    public void SetSliderListener(SliderCallBackDelegate listener)
    {
        mCallBack = listener;
    }

    void SliderValueChange(float v)
    {
        echo.text = v.ToString("0.0000");
        if (mCallBack != null)
        {
            mCallBack(v);
        }
    }

    public float GetValue() { return slider.value; }
    public void SetValue(float v) { slider.value = v; SliderValueChange(v); }
    public void InitSliderRange(float min, float max, float v)
    {
        slider.minValue = min;
        slider.maxValue = max;
        SetValue(v);
    }
}
