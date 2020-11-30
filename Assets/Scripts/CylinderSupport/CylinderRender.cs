using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRender : MonoBehaviour
{
  public GameObject vertexPrefab = null;
  public GameObject meshPrefab = null;
  public float resolutionVert = 30f;
  public float cylinderTotalHeight = 20f;
  private float resolutionDist = 0f;
  private float cylinderRadius = 10;
  private Transform[,] manipulatedVertices = null;

  private float currCylRotation = 360f;
  void Awake()
  {
    resolutionDist = (360f / resolutionVert) * Mathf.Deg2Rad;
  }

  void Start()
  {
    Debug.Assert(vertexPrefab != null);
    Debug.Assert(meshPrefab != null);
  }

  public void SetResolution(float res)
  {
    // maintain height consistency
    resolutionVert = res - 1;
    resolutionDist = (currCylRotation / resolutionVert) * Mathf.Deg2Rad;
    DestroyPreviousRender(true);
    GenerateCylinderVertices();
  }

  public void SetRotation(float rot)
  {
    resolutionDist = (rot / resolutionVert) * Mathf.Deg2Rad;
    currCylRotation = rot;
    for (int height = 0; height <= resolutionVert ; height++)
    {
      float radius =
        (manipulatedVertices[height,0].localPosition - new Vector3(0, manipulatedVertices[height,0].localPosition.y, 0)) .magnitude;
      ManipulateRotationalSweep(height, radius);
    }
  }

  public void DestroyPreviousRender(bool destroyMesh = false)
  {
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
    manipulatedVertices = new Transform[(int)resolutionVert + 1, (int)resolutionVert + 1];
    for (int height = 0; height <= resolutionVert; height++)
    {
      // maintain width consistency
      for (int i = 0; i < resolutionVert + 1; i++)
      {
        Transform vertex = CreateLayer((cylinderTotalHeight / resolutionVert) * height, cylinderRadius, i);
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
    for(int i = 1; i < manipulatedVertices.GetLength(0); i++)
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
    for(int i = 0; i < manipulatedVertices.GetLength(0); i++)
    {
      for(int k = 0; k < manipulatedVertices.GetLength(1); k++)
      {
        manipulatedVertices[i,k].gameObject.SetActive(state);
      }
    }
  } 
}
