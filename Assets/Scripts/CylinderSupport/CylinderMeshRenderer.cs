using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMeshRenderer : MonoBehaviour
{
  public CylinderRender cylinderRender = null;
  public TextureMesh textMesh = null;
  public bool isSelected = false;
  private Mesh cylinderMesh = null;

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
    textMesh.Recalc(cylinderMesh.vertices, cylinderMesh.normals, cylinderMesh, cylinderRender.GetVerticesControllers(), cylinderRender.GetWidth());
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
    int numTriangles = (cylinderWidth - 1) * (cylinderHeight - 1) * 2;
    // each triangle has 3 points
    int[] triangles = new int[numTriangles * 3];

    int temp, curRow, tNdx = 0;
    for (int i = 0; i < vertices.Length; i++)
    {
      if(6 * tNdx >= triangles.Length) continue;
      temp = i;
      curRow = 0;
      while (temp > cylinderWidth - 1)
      {
        temp -= cylinderWidth;
        curRow++;
      }
      if ((temp % cylinderWidth < cylinderWidth - 1) && (curRow < cylinderWidth - 1))
      {
        triangles[6 * tNdx] = i;
        triangles[6 * tNdx + 1] = i + cylinderWidth;
        triangles[6 * tNdx + 2] = i + cylinderWidth + 1;

        triangles[6 * tNdx + 3] = i;
        triangles[6 * tNdx + 4] = i + cylinderWidth + 1;
        triangles[6 * tNdx + 5] = i + 1;

        tNdx++;
      }
    }

    cylinderMesh.vertices = vertices;
    cylinderMesh.triangles = triangles;
  }


}
