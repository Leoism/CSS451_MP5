using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextureMesh : MonoBehaviour
{
    List<GameObject> vControllers = new List<GameObject>();
    Vector3[] Vertices;
    Vector3[] Normals;
    int[] Tris;

    Mesh TheMesh = null;
    Transform Vertex = null, SelectedFrame = null;
    public Transform AxisFrame = null;

    Color selected = new Color(1f, 1f, 0, 1f);
    Color deselect;

    private GameObject targetedObj = null;
    private int fingerID = -1;
    Vector3 mouseDown = Vector3.zero;
    Vector3 dPos = Vector3.zero;

    public SliderWithEcho dRes = null;
    const int RES_MIN = 2;
    const int RES_MAX = 20;

    const float LENGTH = 10f;
    const float OFFSET = -5f;
    int curRes;

    bool isActive = false;

    void Awake()
    {
        if (AxisFrame != null) AxisFrame.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(dRes != null);
        TheMesh = GetComponent<MeshFilter>().mesh;
        TheMesh.Clear();

        for (int i = 0; i < RES_MAX * RES_MAX; i++)
        {
            GameObject g = Instantiate(Resources.Load("Prefabs/Vertices/MeshVertex")) as GameObject;
            vControllers.Add(g);
        }
        dRes.InitSliderRange(RES_MIN, RES_MAX, 5);

        dRes.SetSliderListener(SetVerticies);    
    }

    // Update is called once per frame
    void Update()
    {
        ShowVertices();
        MouseSupport();
        Recalc(TheMesh.vertices, TheMesh.normals, curRes, curRes, TheMesh, vControllers);
    }

    #region DRAW_MESH

    void SetVerticies(float v)
    {
        TheMesh.Clear();
        AxisFrame.gameObject.SetActive(false);
        isActive = false;
        curRes = (int)v;
        Vertices = new Vector3[curRes * curRes];
        Normals = new Vector3[curRes * curRes];

        float offset = LENGTH / (curRes - 1);
        int curRow = 0, temp = 0;
        for(int i = 0; i < Vertices.Length; i++)
        {
            temp = i;
            curRow = 0;
            while(temp > curRes - 1)
            {
                temp -= curRes;
                curRow++;
            }
            Vertices[i] = new Vector3(OFFSET + offset * (temp % curRes), 0, OFFSET + offset * curRow);
            Normals[i] = Vector3.up;
            vControllers[i].transform.position = Vertices[i];
        }
        DrawTris(curRes, curRes);
        Recalc(TheMesh.vertices, TheMesh.normals, curRes, curRes, TheMesh, vControllers);
    }

    void DrawTris(int width, int height)
    {
        Tris = new int[(width - 1) * (height - 1) * 6];
        int triangleIdx = 0;
        for (int row = 0; row < height - 1; row++)
        {
            for (int col = 0; col < width - 1; col++)
            {
                int currPt = row * width + col;
                Tris[triangleIdx++] = currPt;
                Tris[triangleIdx++] = currPt + width;
                Tris[triangleIdx++] = currPt + width + 1;

                Tris[triangleIdx++] = currPt;
                Tris[triangleIdx++] = currPt + width + 1;
                Tris[triangleIdx++] = currPt + 1;
            }
        }
        TheMesh.vertices = Vertices;
        TheMesh.triangles = Tris;
        TheMesh.normals = Normals;
    }

    #endregion

    #region VISUALIZE

    void ShowVertices()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isActive)
            {
                for (int i = 0; i < RES_MAX * RES_MAX; i++)
                {
                    if(i < curRes * curRes)
                    {
                        vControllers[i].SetActive(true);
                    } else
                    {
                        vControllers[i].SetActive(false);
                    }
                    
                }
                isActive = true;
            }
            
        }
        else { 
            HideVertices();
        }
    }

    public void HideVertices()
    {
        for(int i = 0; i < curRes * curRes; i++)
        {
            vControllers[i].SetActive(false);
        }
        isActive = false;
        AxisFrame.gameObject.SetActive(false);
    }

    #endregion

    void MouseSupport()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                    if (SelectedFrame != null) return;
                    
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                    if (hit)
                    {
                        targetedObj = hitInfo.transform.gameObject;
                        if (targetedObj.tag == "Vertex")
                        {
                            Vertex = targetedObj.transform;
                            AxisFrame.gameObject.SetActive(true);
                            AxisFrame.position = Vertex.position;
                        } 
                        else if(targetedObj.tag == "Axis")
                        {
                            SelectedFrame = targetedObj.transform;
                            mouseDown = Input.mousePosition;
                            deselect = targetedObj.GetComponent<Renderer>().material.color;
                            targetedObj.GetComponent<Renderer>().material.color = selected;
                        } else
                        {
                            AxisFrame.gameObject.SetActive(false);
                        }
                    } else
                    {
                        AxisFrame.gameObject.SetActive(false);
                    }
                } else if (Input.GetMouseButton(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject(fingerID)) return;
                    if(SelectedFrame != null)
                    {
                        dPos = (mouseDown - Input.mousePosition) * Time.smoothDeltaTime;
                        mouseDown = Input.mousePosition;
                        if(SelectedFrame.gameObject.name == "FrameX") {
                            Vertex.position += new Vector3(-dPos.x, 0, 0);
                        }
                        if (SelectedFrame.gameObject.name == "FrameY")
                        {
                            Vertex.position += new Vector3(0, -dPos.y, 0);
                        }
                        if (SelectedFrame.gameObject.name == "FrameZ")
                        {
                            Vertex.position += new Vector3(0, 0, -dPos.x);
                        }
                        AxisFrame.position = Vertex.position;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if(SelectedFrame != null)
                {
                    SelectedFrame.gameObject.GetComponent<Renderer>().material.color = deselect;
                    SelectedFrame = null;
                }
            }
        }
    }

    void Recalc(Vector3[] vertices, Vector3[] normals, int width, int height, Mesh theMesh, List<GameObject> vertControllers)
    {
        int numTriangles = (width - 1) * (height - 1) * 2;
        Vector3[] triangleNormals = new Vector3[numTriangles];
        int[] triangleMapping = theMesh.triangles;
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

        theMesh.normals = normals;
        for(int i = 0; i < theMesh.normals.Length; i++)
        {
            vertices[i] = vertControllers[i].transform.localPosition;
            vertControllers[i].transform.up = theMesh.normals[i];
        }
        theMesh.vertices = vertices;
    }

    Vector3 FaceNormal(Vector3[] vertices, int triPt1, int triPt2, int triPt3)
    {
        Vector3 a = vertices[triPt2] - vertices[triPt1];
        Vector3 b = vertices[triPt3] - vertices[triPt1];
        return Vector3.Cross(a, b).normalized;
    }
}
