using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMeshManipulation : MonoBehaviour
{
  public Transform axisFrame = null;
  public CylinderRender cylRenderer = null;
  private Transform currSelected = null;
  private Transform selectedAxis = null;
  private Vector3 prevMousePos;
  private float mouseSpeed = 5f;
  void Start()
  {
    Debug.Assert(axisFrame != null);
    Debug.Assert(cylRenderer != null);
  }
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      SelectMeshVertex();
      SelectAxis();
    }
    else if (Input.GetMouseButton(0))
    {
      MoveAxis();
    } else {
      selectedAxis = null;
    }
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
      }
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
      }
    }
  }

  void MoveAxis()
  {
    if (selectedAxis == null) return;
    string axisName = selectedAxis.gameObject.name;
    if (axisName.Equals("FrameX"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.right);
      axisFrame.localPosition += (offset > 0 ? Vector3.right : -Vector3.right) * mouseSpeed * Time.smoothDeltaTime;
    }
    else if (axisName.Equals("FrameY"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.up);
      axisFrame.localPosition += (offset > 0 ? Vector3.up : -Vector3.up) * mouseSpeed * Time.smoothDeltaTime;
    }
    else if (axisName.Equals("FrameZ"))
    {
      float offset = Vector3.Dot(Input.mousePosition - prevMousePos, Vector3.forward);
      axisFrame.localPosition += (offset > 0 ? Vector3.forward : -Vector3.forward) * mouseSpeed * Time.smoothDeltaTime;
    }
    currSelected.localPosition = axisFrame.localPosition;
    cylRenderer.UnknownRotationalSweep(currSelected.gameObject);
  }
}
