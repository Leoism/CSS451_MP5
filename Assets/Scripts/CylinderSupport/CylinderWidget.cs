using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderWidget : MonoBehaviour
{
  public CylinderRender verticesRenderer = null;
  public SliderWithEcho resolution = null;
  public SliderWithEcho rotation = null;
  // Start is called before the first frame update
  void Start()
  {
    Debug.Assert(verticesRenderer != null);
    Debug.Assert(resolution != null);
    Debug.Assert(rotation != null);
    resolution.SetSliderListener(RenderCylinderVertices);
    rotation.SetSliderListener(SetCylinderRotation);
  }

  private void RenderCylinderVertices(float res)
  {
    verticesRenderer.SetResolution(res);
  }

  private void SetCylinderRotation(float rot)
  {
    verticesRenderer.SetRotation(rot);
  }
}
