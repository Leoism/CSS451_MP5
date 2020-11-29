using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class XformControl : MonoBehaviour
{
    public Toggle T, R, S;
    public SliderWithEcho X, Y, Z;
    public Text ShowName = null;

    #region CONSTS
    const float ROTATION_MAX = 180f;
    const float ROTATION_MIN = -180f;

    const float POS_MAX = 4f;
    const float POS_MIN = -4f;

    const float SCALE_MAX = 10f;
    const float SCALE_MIN = 0.1f;
    #endregion

    private Transform selected;
    public Transform AxisFrame;
    private Vector3 prevSliderVal = Vector3.zero;

    void Awake()
    {
        if(AxisFrame != null)
        {
            AxisFrame.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(ShowName != null);

        Debug.Assert(X != null);
        Debug.Assert(Y != null);
        Debug.Assert(Z != null);

        T.onValueChanged.AddListener(LookTranslation);
        R.onValueChanged.AddListener(LookRotation);
        S.onValueChanged.AddListener(LookScale);

        X.SetSliderListener(ChangeX);
        Y.SetSliderListener(ChangeY);
        Z.SetSliderListener(ChangeZ);

        T.isOn = true;
        R.isOn = false;
        S.isOn = false;
        LookTranslation(true);
    }

    #region LOOK_SLIDER_VALUES
    void LookTranslation(bool v)
    {
        if (v)
        {
            Vector3 p = ReadXform();
            prevSliderVal = p;
            X.InitSliderRange(POS_MIN, POS_MAX, p.x);
            Y.InitSliderRange(POS_MIN, POS_MAX, p.y);
            Z.InitSliderRange(POS_MIN, POS_MAX, p.z);
        }
    }

    void LookRotation(bool v)
    {
        if (v)
        {
            Vector3 r = ReadXform();
            prevSliderVal = r;
            X.InitSliderRange(0, 0, 0);
            Y.InitSliderRange(0, 0, 0);
            Z.InitSliderRange(ROTATION_MIN, ROTATION_MAX, r.z);
            prevSliderVal = r;
        }
    }

    void LookScale(bool v)
    {
        if (v)
        {
            Vector3 s = ReadXform();
            prevSliderVal = s;
            X.InitSliderRange(SCALE_MIN, SCALE_MAX, s.x);
            Y.InitSliderRange(SCALE_MIN, SCALE_MAX, s.y);
            Z.InitSliderRange(1f, 1f, 1f);
        }
    }
    #endregion

    #region RESPOND_SLIDER_CHANGE
    void ChangeX(float v)
    {
        Vector3 p = ReadXform();
        float dx = v - prevSliderVal.x;
        prevSliderVal.x = v;
        Quaternion q = Quaternion.AngleAxis(dx, Vector3.right);
        p.x = v;
        UISetXform(ref p, ref q);
    }

    void ChangeY(float v)
    {
        Vector3 p = ReadXform();
        float dy = v - prevSliderVal.y;
        prevSliderVal.y = v;
        Quaternion q = Quaternion.AngleAxis(dy, Vector3.up);
        p.y = v;
        UISetXform(ref p, ref q);
    }

    void ChangeZ(float v)
    {
        Vector3 p = ReadXform();
        float dz = v - prevSliderVal.z;
        prevSliderVal.z = v;
        Quaternion q = Quaternion.AngleAxis(dz, Vector3.forward);
        p.z = v;
        UISetXform(ref p, ref q);
    }
    #endregion

    #region UTILITIES
    public void SetSelected(Transform xform)
    {
        if (selected != null)
        {
            
        }
        selected = xform;
        prevSliderVal = Vector3.zero;
        if (xform != null)
        {
            ShowName.text = "Selected: " + selected.name;
        }
        else
        {
            ShowName.text = "Selected: None";
        }

        SetUI();
    }

    public void SetUI()
    {
        Vector3 p = ReadXform();
        X.SetValue(p.x);
        Y.SetValue(p.y);
        Z.SetValue(p.z);
    }

    private Vector3 ReadXform()
    {
        Vector3 p = Vector3.zero;

        if (T.isOn)
        {
            if(selected != null)
            {
                p = selected.localPosition;
            }
            else
            {
                p = Vector3.zero;
            }
        } else if (S.isOn)
        {
            if (selected != null)
            {
                p = selected.localScale;
            }
            else
            {
                p = Vector3.zero;
            }
        } 
        else if (R.isOn)
        {
            p = Vector3.zero;
        }
        return p;
    }

    private void UISetXform(ref Vector3 p, ref Quaternion q)
    {
        if (selected == null) return;
        if (T.isOn)
        {
            selected.localPosition = p;
        } else if (S.isOn)
        {
            selected.localScale = p;
        } else if (R.isOn)
        {
            selected.localRotation *= q;
        }
    }

    #endregion
}
