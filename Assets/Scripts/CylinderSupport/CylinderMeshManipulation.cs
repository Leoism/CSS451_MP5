﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CylinderMeshManipulation : MonoBehaviour
{
  public Transform axisFrame = null;
  public CylinderRender cylRenderer = null;
  public CylinderMeshRenderer cylMeshRender = null;

  private bool isSelected = false;
  private Transform currSelected = null;
  private Transform selectedAxis = null;
  private Color axisColor;
  private Vector3 prevMousePos;
  private float mouseSpeed = 2f;
    private int fingerID = -1;
  void Start()
  {
    Debug.Assert(axisFrame != null);
    Debug.Assert(cylRenderer != null);
    Debug.Assert(cylMeshRender != null);
  }
  void Update()
  {
    if (!isSelected) return;
    if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
    {
            if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
            SelectMeshVertex();
      SelectAxis();
    }
    else if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftAlt))
    {
            if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
            if (selectedAxis != null)
        selectedAxis.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
      MoveAxis();
    } else {
      if (selectedAxis != null)
        selectedAxis.gameObject.GetComponent<Renderer>().material.color = axisColor;
      selectedAxis = null;
    }
    if (Input.GetKey(KeyCode.LeftControl))
    {
      ToggleVertices(true);
    } else
    {
      ToggleVertices(false);
    }
  }

  public void SetSelect(bool isSelected)
  {
    this.isSelected = isSelected;
    if (!isSelected)
    {
      axisFrame.gameObject.SetActive(false);
      cylRenderer.DestroyPreviousRender(true);
    }
    else cylRenderer.GenerateCylinderVertices();
    cylMeshRender.SetSelected(isSelected);
  }
    void ToggleVertices(bool state)
    {
        if (!isSelected && state) return;
        cylRenderer.ToggleAllVertices(state);
        if (!state) currSelected = null;
        if (state && currSelected != null)
        {
            axisFrame.gameObject.SetActive(state);
        }
        else axisFrame.gameObject.SetActive(false); 
    }



  void SelectMeshVertex()
  {
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit))
    {
      string hitTag = hit.transform.gameObject.tag;
      if (hitTag.Equals("MeshVertex"))
      {
        currSelected = hit.transform;
        axisFrame.localPosition = currSelected.localPosition;
        prevMousePos = Input.mousePosition;
        axisFrame.gameObject.SetActive(true);
      }
        }
        else
        {
            currSelected = null;
            axisFrame.gameObject.SetActive(false);
        }
  }

  void SelectAxis()
  {
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit))
    {
      string hitName = hit.transform.gameObject.name;
      if (hitName.Equals("FrameX") || hitName.Equals("FrameY") || hitName.Equals("FrameZ"))
      {
        selectedAxis = hit.transform;        
        axisColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
      }
    }
  }

  void MoveAxis()
  {
    if (selectedAxis == null) return;
        if (currSelected == null) return;
    string axisName = selectedAxis.gameObject.name;
    if (axisName.Equals("FrameX"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.right);
      prevMousePos = Input.mousePosition;
      axisFrame.localPosition += (offset * Vector3.right) * mouseSpeed * Time.smoothDeltaTime;
    }
    else if (axisName.Equals("FrameY"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.up);
      prevMousePos = Input.mousePosition;
      axisFrame.localPosition += (offset * Vector3.up) * mouseSpeed * Time.smoothDeltaTime;
    }
    else if (axisName.Equals("FrameZ"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.right);
      prevMousePos = Input.mousePosition;
      Vector3 newPos = new Vector3(0, 0, (offset * Vector3.right).x);
      axisFrame.localPosition +=  newPos * mouseSpeed * Time.smoothDeltaTime;  
    }

    currSelected.localPosition = axisFrame.localPosition;
    cylRenderer.UnknownRotationalSweep(currSelected.gameObject);
  }
}
