using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class XformControl : MonoBehaviour
{
    public TextureMesh mesh = null;
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
            Vector2 p = mesh.GetT();
            prevSliderVal = p;
            X.InitSliderRange(POS_MIN, POS_MAX, p.x);
            Y.InitSliderRange(POS_MIN, POS_MAX, p.y);
            Z.InitSliderRange(0, 0, 0);
        }
    }

    void LookRotation(bool v)
    {
        if (v)
        {
            float r = mesh.getR();
            prevSliderVal = new Vector3(0, 0, r);
            X.InitSliderRange(0, 0, 0);
            Y.InitSliderRange(0, 0, 0);
            Z.InitSliderRange(ROTATION_MIN, ROTATION_MAX, r);
            prevSliderVal = new Vector3(0, 0, r);
        }
    }

    void LookScale(bool v)
    {
        if (v)
        {
            Vector3 s = mesh.GetS();
            prevSliderVal = s;
            X.InitSliderRange(SCALE_MIN, SCALE_MAX, s.x);
            Y.InitSliderRange(SCALE_MIN, SCALE_MAX, s.y);
            Z.InitSliderRange(0, 0, 0);
        }
    }
    #endregion

    #region RESPOND_SLIDER_CHANGE
    void ChangeX(float v)
    {
        
        if (T.isOn)
        {
            Vector2 t = new Vector2(v, Y.GetValue());
            mesh.SetT(t);
        } else if (S.isOn) 
        {
            Vector2 s = new Vector2(v, Y.GetValue());
            mesh.SetS(s);
        }
    }

    void ChangeY(float v)
    {
        if (T.isOn)
        {
            Vector2 t = new Vector2(X.GetValue(), v);
            mesh.SetT(t);
        } else if (S.isOn) 
        {
            Vector2 s = new Vector2(X.GetValue(), v);
            mesh.SetS(s);
        }
    }

    void ChangeZ(float v)
    {
        if (R.isOn)
        {
            mesh.SetR(v);
        }
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
