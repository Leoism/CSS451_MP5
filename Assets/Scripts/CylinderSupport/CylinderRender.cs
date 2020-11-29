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
  private Transform[] manipulatedVertices = null;

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

  public void GenerateCylinderVertices()
  {
    manipulatedVertices = new Transform[(int)resolutionVert + 1];
    for (int height = 0; height <= resolutionVert; height++)
    {
      // maintain width consistency
      for (int i = 0; i < resolutionVert + 1; i++)
      {
        Transform vertex = CreateLayer((cylinderTotalHeight / resolutionVert) * height, cylinderRadius, i);
        if (i == 0) manipulatedVertices[height] = vertex;
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

  private void RotationalSweep(Vector3 vertexPos)
  {
    for (int vertex = 0; vertex < resolutionVert + 1; vertex++)
    {
      float radius = (vertexPos - new Vector3(0, vertexPos.y, 0)) .magnitude;
      CreateLayer(vertexPos.y, radius, vertex);
    }
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
    DestroyPreviousRender();
    for (int height = 0; height <= resolutionVert ; height++)
    {
      RotationalSweep(manipulatedVertices[height].localPosition);
    }
  }

  private void DestroyPreviousRender(bool destroyMesh = false)
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

  public Transform[] GetMeshTransforms()
  {
    return manipulatedVertices;
  }
}
