using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderWidget : MonoBehaviour
{
  public CylinderRender verticesRenderer = null;
  public SliderWithEcho widthResoluion = null;
  public SliderWithEcho heightResolution = null;
  public SliderWithEcho rotation = null;
  // Start is called before the first frame update
  void Start()
  {
    Debug.Assert(verticesRenderer != null);
    Debug.Assert(widthResoluion != null);
    Debug.Assert(rotation != null);
    widthResoluion.SetSliderListener(ChangeWidthResolution);
    heightResolution.SetSliderListener(ChangeHeightResolution);
    rotation.SetSliderListener(SetCylinderRotation);
    verticesRenderer.SetResolution(widthResoluion.GetValue(), heightResolution.GetValue());
    verticesRenderer.SetRotation(rotation.GetValue());
  }

  private void ChangeWidthResolution(float res)
  {
    verticesRenderer.SetWidth(res);
  }

  private void ChangeHeightResolution(float res)
  {
    verticesRenderer.SetHeight(res);
  }

  private void SetCylinderRotation(float rot)
  {
    verticesRenderer.SetRotation(rot);
  }
}
