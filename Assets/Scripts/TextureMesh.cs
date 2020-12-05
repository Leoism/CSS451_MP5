using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextureMesh : MonoBehaviour
{
    public float rotation = 0f;
    public float scaleX = 1f;
    public float scaleZ = 1f;
    public float translateX = 0f;
    public float translateZ = 0f;

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

    public SliderWithEcho wResSlider = null;
    public SliderWithEcho hResSlider = null;
    const int RES_MIN = 2;
    const int RES_MAX = 20;

    const float LENGTH = 10f;
    const float OFFSET = -5f;
    int hRes = 5;
    int wRes = 5;

    bool isActive = false;

    void Awake()
    {
        if (AxisFrame != null) AxisFrame.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(hResSlider != null);
        Debug.Assert(wResSlider != null);
        TheMesh = GetComponent<MeshFilter>().mesh;
        TheMesh.Clear();

        for (int i = 0; i < RES_MAX * RES_MAX; i++)
        {
            GameObject g = Instantiate(Resources.Load("Prefabs/Vertices/MeshVertex")) as GameObject;
            vControllers.Add(g);
        }

        wResSlider.InitSliderRange(RES_MIN, RES_MAX, 5);
        hResSlider.InitSliderRange(RES_MIN, RES_MAX, 5);

        wResSlider.SetSliderListener(SetWidth);    
        hResSlider.SetSliderListener(SetHeight);
    }

    // Update is called once per frame
    void Update()
    {
        ShowVertices();
        MouseSupport();
        Recalc(TheMesh.vertices, TheMesh.normals, wRes, hRes, TheMesh, vControllers);
        Vector2 T = new Vector2(translateX, translateZ);
        Vector2 S = new Vector2(scaleX, scaleZ);
        Vector2[] uv = TheMesh.uv;
        for(int i = 0; i < TheMesh.uv.Length; i++)
        {
            uv[i] += Matrix3x3Helpers.CreateTRS(T, rotation, S) * uv[i];
        }
        TheMesh.uv = uv;
    }

    public void SetT(Vector2 t)
    {
        translateX = t.x * 20;
        translateZ = t.y * 20;
        Vector2 T = new Vector2(translateX, translateZ);
        Vector2[] uv = TheMesh.uv;
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += Matrix3x3Helpers.CreateTranslation(T) * uv[i];
        }
        TheMesh.uv = uv;
    }

    public Vector2 GetT() { return new Vector2(translateX, translateZ); }

    public void SetR(float r)
    {
        rotation = r;
        Vector2[] uv = TheMesh.uv;
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += Matrix3x3Helpers.CreateRotation(rotation) * uv[i];
        }
        TheMesh.uv = uv;
    }

    public float getR() { return rotation; }

    public void SetS(Vector2 s)
    {
        scaleX = s.x;
        scaleZ = s.y;
        Vector2 S = new Vector2(scaleX, scaleZ);
        Vector2[] uv = TheMesh.uv;
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] += Matrix3x3Helpers.CreateScale(S) * uv[i];
        }
        TheMesh.uv = uv;
    }

    public Vector2 GetS() { return new Vector2(scaleX, scaleZ); }

    #region DRAW_MESH

    void SetWidth(float width)
    {
        wRes = (int)width;
        SetVerticies(wRes, hRes);
    }

    void SetHeight(float height)
    {
        hRes = (int)height;
        SetVerticies(wRes,hRes);
    }
    void SetVerticies(float width, float height)
    {
        TheMesh.Clear();
        AxisFrame.gameObject.SetActive(false);
        isActive = false;
        wRes = (int)width;
        hRes = (int)height;
        Vertices = new Vector3[wRes * hRes];
        Normals = new Vector3[wRes * hRes];

        float wOffset = LENGTH / (wRes - 1);
        float hOffset = LENGTH / (hRes - 1);
        int idx = 0;

        for (int i = 0; i < hRes; i++)
        {
            for (int j = 0; j < wRes; j++) {
                Vertices[idx] = new Vector3((float)(-1 + (j * wOffset)), 0, (float)(-1 + (i * hOffset)));
                Normals[idx] = Vector3.up;
                vControllers[idx].transform.localPosition = Vertices[idx];
                idx++;
            }
        }
        DrawTris(wRes, hRes);
        Recalc(TheMesh.vertices, TheMesh.normals, wRes, hRes, TheMesh, vControllers);
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
                    if(i < wRes * hRes)
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
        for(int i = 0; i < wRes * hRes; i++)
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

    void LoadUVs(Vector3[] vertices, Mesh theMesh)
    {
        Vector2[] calcUV = new Vector2[vertices.Length];
        for(int i = 0; i < vertices.Length; i++)
        {
            calcUV[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        theMesh.uv = calcUV;
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
        LoadUVs(vertices, theMesh);
    }

    Vector3 FaceNormal(Vector3[] vertices, int triPt1, int triPt2, int triPt3)
    {
        Vector3 a = vertices[triPt2] - vertices[triPt1];
        Vector3 b = vertices[triPt3] - vertices[triPt1];
        return Vector3.Cross(a, b).normalized;
    }
}
