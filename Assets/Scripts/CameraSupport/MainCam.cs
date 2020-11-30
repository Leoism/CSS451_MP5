using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCam : MonoBehaviour
{
    public Transform LookPoint = null;

    Vector3 LookPos = Vector3.zero;
    Vector3 LookOffset = Vector3.zero;

    Camera TheCamera = null;

    const float TUMBLE_LOCK = 75f;
    const float ZOOM_MIN = 0.5f;

    private int fingerID = -1;

    Vector3 dTrack = Vector3.zero;
    Vector3 dTumble = Vector3.zero;

    Vector3 mouseDown = Vector3.zero;

    void Awake()
    {
        TheCamera = transform.gameObject.GetComponent<Camera>();
        Debug.Assert(TheCamera != null);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(LookPoint != null);
        LookPoint.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMouseEvents();
        SetCameraRotation();
    }

    #region MOVE_CAMERA

    void ProcessMouseEvents()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            ChangeFOV(Input.GetAxis("Mouse ScrollWheel"));
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                mouseDown = Input.mousePosition;

                dTumble = Vector3.zero;
                dTrack = Vector3.zero;
            }
            if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                //if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                dTumble = mouseDown - Input.mousePosition;
                dTrack = mouseDown - Input.mousePosition;
                mouseDown = Input.mousePosition;

                if (Input.GetMouseButton(0))
                {
                    ProcessTumble(dTumble);
                }
                if (Input.GetMouseButton(1))
                {
                    ProcessTrack(dTrack * Time.smoothDeltaTime);
                }
            }
        }
    }

    void ChangeFOV(float dFOV)
    {
        Vector3 v = LookPoint.localPosition - transform.localPosition;
        float dist = v.magnitude;
        dist -= dFOV * 2f;
        if(dist < ZOOM_MIN && dFOV > 0)
        {
            return;
        }
        transform.localPosition = LookPoint.localPosition - dist * v.normalized;
    }

    void ProcessTrack(Vector3 delta)
    {
        Vector3 oldPos = transform.localPosition;
        transform.Translate(delta);
        LookPoint.localPosition += transform.localPosition - oldPos;
        LookPos += transform.localPosition - oldPos;
    }

    void ProcessTumble(Vector3 delta)
    {
        Quaternion q1 = Quaternion.AngleAxis(delta.y, transform.right);
        Quaternion q2 = Quaternion.AngleAxis(-delta.x, transform.up);

        Matrix4x4 r = Matrix4x4.Rotate(q2);
        Matrix4x4 invP = Matrix4x4.TRS(-LookPoint.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        Vector3 newPos = r.MultiplyPoint(transform.localPosition);
        transform.localPosition = newPos;

        //DO NOT DO IF ANGLE BETWEEN AXISFRAME XZ PLANE AND LOOK DIRECTION > TUMBLE_LOCK
        Vector3 curPos = LookPoint.localPosition - transform.localPosition;
        if(Mathf.Abs(Vector3.Dot(curPos.normalized,LookPoint.up)) < .9848f)
        {
            r = Matrix4x4.Rotate(q1);
            invP = Matrix4x4.TRS(-LookPoint.localPosition, Quaternion.identity, Vector3.one);
            r = invP.inverse * r * invP;
            newPos = r.MultiplyPoint(transform.localPosition);
            transform.localPosition = newPos;
        }
    }

    void SetCameraRotation()
    {
        Vector3 V = LookPoint.localPosition - transform.localPosition;
        transform.up = Vector3.up;
        transform.forward = V;
    }

    #endregion
}
