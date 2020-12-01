using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public XformControl meshXform = null;
    public Transform TheCylinder = null, AxisFrame = null;
    public TextureMesh TheMesh = null;
    public CylinderMeshManipulation cylinderMesh = null;
    Transform curSelected = null;

    public Dropdown TheMenu = null;
    public Camera mainCamera = null;
    public Transform lookAt = null;
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    List<Transform> mSelectedTransform = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(TheMenu != null);
        Debug.Assert(meshXform != null);
        Debug.Assert(AxisFrame != null);
        Debug.Assert(TheMesh != null);
        Debug.Assert(TheCylinder != null);
        Debug.Assert(cylinderMesh != null);
        Debug.Assert(mainCamera != null);
        Debug.Assert(lookAt != null);

        meshXform.SetSelected(TheMesh.gameObject.transform);
        mSelectMenuOptions.Add(new Dropdown.OptionData("Mesh"));
        mSelectMenuOptions.Add(new Dropdown.OptionData("Cylinder"));
        mSelectedTransform.Add(TheMesh.gameObject.transform);
        mSelectedTransform.Add(TheCylinder);

        TheMenu.AddOptions(mSelectMenuOptions);
        TheMenu.onValueChanged.AddListener(ShowMesh);
        curSelected = TheMesh.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowMesh(int index)
    {
        TheMesh.HideVertices();
        AxisFrame.gameObject.SetActive(false);

        Transform t = mSelectedTransform[index];
        if(t != null)
        {
            t.gameObject.SetActive(true);
            if(curSelected != null)
            {
                curSelected.gameObject.SetActive(false);
            }
            curSelected = t;
            // 1 is cylinder
            if(index == 1)
            {
                cylinderMesh.SetSelect(true);
                mainCamera.transform.localPosition = 
                    new Vector3(25, 25, -10);
                lookAt.localPosition = 
                    new Vector3(-1, 5, 2);
                
            } else
            {
                mainCamera.transform.localPosition =
                    new Vector3(0, 3, -12);
                lookAt.localPosition =
                    new Vector3(0, -1.7f, 7);
                cylinderMesh.SetSelect(false);
            }
        }
    }

    public Transform GetCurSelected()
    {
        return curSelected;
    }
}
