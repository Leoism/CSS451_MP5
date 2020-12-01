using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMeshRenderer : MonoBehaviour
{
  public CylinderRender cylinderRender = null;
  public TextureMesh textMesh = null;
  public bool isSelected = false;
  private Mesh cylinderMesh = null;
  private int numTriangles = 0;

  // Start is called before the first frame update
  void Start()
  {
    cylinderMesh = GetComponent<MeshFilter>().mesh;
    if(!isSelected) return;
    Debug.Assert(cylinderRender != null);
    Debug.Assert(textMesh != null);
  }

  // Update is called once per frame
  void Update()
  {
    if(!isSelected) return;
    CreateTriangles();
    CalculateNormals(cylinderMesh.vertices, cylinderMesh.normals, cylinderRender.GetWidth(), cylinderRender.GetHeight());
  }

  public void SetSelected(bool state)
  {
    isSelected = state;
  }

  void CreateTriangles()
  {
    Vector3[] vertices = cylinderRender.GetVertices();
    cylinderMesh.Clear();
    int cylinderWidth = cylinderRender.GetWidth();
    int cylinderHeight = cylinderRender.GetHeight();
    numTriangles = (cylinderWidth - 1) * (cylinderHeight - 1) * 2;
    // each triangle has 3 points
    int[] triangles = new int[numTriangles * 3];
    int triangleIdx = 0;
    for (int row = 0; row < cylinderHeight - 1; row++)
    {
      for (int col = 0; col < cylinderWidth - 1; col++)
      {
        int currPt = row * cylinderWidth + col;
        triangles[triangleIdx++] = currPt;
        triangles[triangleIdx++] = currPt + cylinderWidth;
        triangles[triangleIdx++] = currPt + cylinderWidth + 1;

        triangles[triangleIdx++] = currPt;
        triangles[triangleIdx++] = currPt + cylinderWidth + 1;
        triangles[triangleIdx++] = currPt + 1;
      }
    }

    cylinderMesh.vertices = vertices;
    cylinderMesh.triangles = triangles;
  }

  void CalculateNormals(Vector3[] vertices, Vector3[] normals, int width, int height)
  {
    Vector3[] triangleNormals = new Vector3[numTriangles];
    int[] triangleMapping = cylinderMesh.triangles;
    int verticeIdx = 0;
    for (int tri = 0; tri < triangleNormals.Length; tri++)
    {
      triangleNormals[tri] = 
        FaceNormal(vertices, triangleMapping[verticeIdx], triangleMapping[verticeIdx+1], triangleMapping[verticeIdx+2]);
      verticeIdx += 3;
    }
    
    int edgePt = 0;
    int trianglesPerRow = numTriangles / (height - 1);
    for (int tri = 0; tri < normals.Length; tri++)
    {
      int nextPt = edgePt + 1;
      // first and last triangle
      if (edgePt == 0 || edgePt == trianglesPerRow * (height) - 1)
      {
        Vector3 tri1 = edgePt == 0 ?
          (triangleNormals[edgePt] + triangleNormals[edgePt + 1]).normalized
          : (triangleNormals[edgePt - trianglesPerRow] + triangleNormals[edgePt - 1 - trianglesPerRow]);
        normals[tri] = tri1;
      }
      // top left and bottom right corner
      else if ((edgePt == trianglesPerRow - 1) || (edgePt == trianglesPerRow * (height - 1)))
      {
        int tri1 = edgePt == trianglesPerRow - 1 ?
          edgePt
          : edgePt - trianglesPerRow;
        normals[tri] = (triangleNormals[tri1]).normalized;
      }
      // left and right edges
      else if (edgePt % trianglesPerRow == 0 || nextPt % trianglesPerRow == 0)
      {
        bool isLeft = edgePt % trianglesPerRow == 0;
        normals[tri] = isLeft ?
          (triangleNormals[edgePt] + triangleNormals[edgePt + 1] + triangleNormals[edgePt - trianglesPerRow]).normalized
          : (triangleNormals[edgePt] + triangleNormals[edgePt - trianglesPerRow] + triangleNormals[edgePt - trianglesPerRow - 1]).normalized;
      }
      // top and bottom edges
      else if ((0 < edgePt && edgePt < trianglesPerRow) || ((trianglesPerRow * (height)) - trianglesPerRow < edgePt && edgePt < trianglesPerRow * height))
      {
        bool isBottom = (0 < tri && tri < width);
        int tri1 = isBottom ?
          edgePt : edgePt - trianglesPerRow;
        normals[tri] = (triangleNormals[tri1] + triangleNormals[tri1 + 1] + triangleNormals[tri1 + 2]).normalized;
        edgePt++;
      }
      // everything else
      else
      {
        normals[tri] =
          (triangleNormals[edgePt] + triangleNormals[edgePt + 1]
          + triangleNormals[edgePt + 2] + triangleNormals[edgePt - trianglesPerRow]
          + triangleNormals[edgePt - trianglesPerRow - 1]
          + triangleNormals[edgePt - trianglesPerRow + 1]).normalized;
        edgePt++;
      }
      edgePt++;
    }

    cylinderMesh.normals = normals;
    for(int i = 0; i < cylinderMesh.normals.Length; i++)
    {
      cylinderRender.GetVerticesControllers()[i].transform.up = cylinderMesh.normals[i];

    }
    cylinderMesh.vertices = vertices;
  }

  Vector3 FaceNormal(Vector3[] vertices, int triPt1, int triPt2, int triPt3)
  {
    Vector3 a = vertices[triPt2] - vertices[triPt1];
    Vector3 b = vertices[triPt3] - vertices[triPt1];
    return Vector3.Cross(a, b).normalized;
  }

}
