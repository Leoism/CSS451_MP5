using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRender : MonoBehaviour
{
  public GameObject vertexPrefab = null;
  public GameObject meshPrefab = null;
  public float wResolution = 4f;
  public float hResolution = 4f;
  public float cylinderTotalHeight = 20f;
  private float resolutionDist = 0f;
  private float cylinderRadius = 10;
  private Transform[,] manipulatedVertices = null;

  private float currCylRotation = 180f;
  void Awake()
  {
    resolutionDist = (currCylRotation / wResolution) * Mathf.Deg2Rad;
  }

  void Start()
  {
    Debug.Assert(vertexPrefab != null);
    Debug.Assert(meshPrefab != null);
  }

  public Vector3[] GetVertices()
  {
    Vector3[] allVertices = new Vector3[manipulatedVertices.Length];
    int hLength = manipulatedVertices.GetLength(0);
    int wLength = manipulatedVertices.GetLength(1);
    int i = 0;
    for (int r = 0; r < hLength; r++)
    {
      for (int c = 0; c < wLength; c++)
      {
        allVertices[i++] = manipulatedVertices[r,c].localPosition;
      }
    }
    return allVertices;
  }

  public List<GameObject> GetVerticesControllers()
  {
    GameObject[] allVertices = new GameObject[manipulatedVertices.Length];
    int hLength = manipulatedVertices.GetLength(0);
    int wLength = manipulatedVertices.GetLength(1);
    int i = 0;
    for (int r = 0; r < hLength; r++)
    {
      for (int c = 0; c < wLength; c++)
      {
        allVertices[i++] = manipulatedVertices[r,c].gameObject;
      }
    }
    return new List<GameObject>(allVertices);
  }

  public int GetWidth()
  {
    return manipulatedVertices.GetLength(1);
  }

  public int GetHeight()
  {
    return manipulatedVertices.GetLength(0);
  }

  public void SetResolution(float wRes, float hRes)
  {
    // maintain height consistency
    hResolution = hRes - 1;
    wResolution = wRes - 1;
    resolutionDist = (currCylRotation / wRes) * Mathf.Deg2Rad;
    DestroyPreviousRender(true);
    GenerateCylinderVertices();
  }

  public void SetWidth(float res)
  {
    wResolution = res - 1;
    resolutionDist = (currCylRotation / wResolution) * Mathf.Deg2Rad;
    DestroyPreviousRender(true);
    GenerateCylinderVertices();
  }

  public void SetHeight(float res)
  {
    hResolution = res - 1;
    DestroyPreviousRender(true);
    GenerateCylinderVertices();
  }
  public void SetRotation(float rot)
  {
    if (manipulatedVertices == null) return;
    resolutionDist = (rot / wResolution) * Mathf.Deg2Rad;
    currCylRotation = rot;
    for (int height = 0; height <= hResolution ; height++)
    {
      float radius =
        (manipulatedVertices[height,0].localPosition - new Vector3(0, manipulatedVertices[height,0].localPosition.y, 0)) .magnitude;
      ManipulateRotationalSweep(height, radius);
    }
  }

  public void DestroyPreviousRender(bool destroyMesh = false)
  {
    ToggleAllVertices(true);
    GameObject[] vertices = GameObject.FindGameObjectsWithTag("CylinderVertex");
    foreach (GameObject vertex in vertices)
    {
      Destroy(vertex);
    }
    if (destroyMesh)
    {
      GameObject[] meshVerts = GameObject.FindGameObjectsWithTag("MeshVertex");
      foreach (GameObject vertex in meshVerts)
      {
        Destroy(vertex);
      }
    }
  }

  public void GenerateCylinderVertices()
  {
    manipulatedVertices = new Transform[(int)hResolution + 1, (int)wResolution + 1];
    for (int height = 0; height <= hResolution; height++)
    {
      // maintain width consistency
      for (int i = 0; i < wResolution + 1; i++)
      {
        Transform vertex = CreateLayer((cylinderTotalHeight / hResolution) * height, cylinderRadius, i);
        manipulatedVertices[height,i] = vertex;
      }
    }
  }

  private Transform CreateLayer(float yElevation, float radius, int count)
  {
    Vector3 vertSize = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 vertPos;
    /** Set Vertex position vector. */
    vertPos.x = radius * Mathf.Cos(count * resolutionDist);
    vertPos.y = yElevation;
    vertPos.z = radius * Mathf.Sin(count * resolutionDist);
    /*********************************/
    GameObject vertex = count == 0 ? Instantiate(meshPrefab) : Instantiate(vertexPrefab);
    vertex.tag = count == 0 ? "MeshVertex" : "CylinderVertex";
    vertex.transform.localScale = vertSize;
    vertex.transform.localPosition = vertPos;
    return vertex.transform;
  }

  public void UnknownRotationalSweep(GameObject vertex)
  {
    for (int i = 0; i < manipulatedVertices.GetLength(0); i++)
    {
      if (!manipulatedVertices[i,0].gameObject.Equals(vertex)) continue;
      float radius =
        (manipulatedVertices[i,0].localPosition - new Vector3(0, manipulatedVertices[i,0].localPosition.y, 0)) .magnitude;
      ManipulateRotationalSweep(i, radius);
      break;
    }
  }

  private void ManipulateRotationalSweep(int layer, float radius)
  {
    Transform vertex = manipulatedVertices[layer, 0];
    for(int i = 1; i < manipulatedVertices.GetLength(1); i++)
    {
      Vector3 vertPos;
      /** Set Vertex position vector. */
      vertPos.x = radius * Mathf.Cos(i * resolutionDist);
      vertPos.y = vertex.localPosition.y;
      vertPos.z = radius * Mathf.Sin(i * resolutionDist);
      /*********************************/
      manipulatedVertices[layer,i].localPosition = vertPos;
    }
  }

  public void ToggleAllVertices(bool state)
  {
    if (manipulatedVertices == null) return;
    for(int i = 0; i < manipulatedVertices.GetLength(0); i++)
    {
      for(int k = 0; k < manipulatedVertices.GetLength(1); k++)
      {
        if(manipulatedVertices[i,k].gameObject != null)
        manipulatedVertices[i,k].gameObject.SetActive(state);
      }
    }
  } 
}
