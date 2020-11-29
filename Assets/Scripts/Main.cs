using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public XformControl meshXform = null;
    public Transform TheMesh = null;
    public Transform TheCylinder = null;
    Transform curSelected = null;

    public Dropdown TheMenu = null;
    List<Dropdown.OptionData> mSelectMenuOptions = new List<Dropdown.OptionData>();
    List<Transform> mSelectedTransform = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(TheMenu != null);
        Debug.Assert(meshXform != null);

        Debug.Assert(TheMesh != null);
        Debug.Assert(TheCylinder != null);

        meshXform.SetSelected(TheMesh);
        mSelectMenuOptions.Add(new Dropdown.OptionData("Mesh"));
        mSelectMenuOptions.Add(new Dropdown.OptionData("Cylinder"));
        mSelectedTransform.Add(TheMesh);
        mSelectedTransform.Add(TheCylinder);

        TheMenu.AddOptions(mSelectMenuOptions);
        TheMenu.onValueChanged.AddListener(ShowMesh);
        curSelected = TheMesh;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowMesh(int index)
    {
        Transform t = mSelectedTransform[index];
        if(t != null)
        {
            t.gameObject.SetActive(true);
            if(curSelected != null)
            {
                curSelected.gameObject.SetActive(false);
            }
            curSelected = t;
        }
    }
}
