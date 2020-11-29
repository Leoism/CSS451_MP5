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
    int verNdx = -1;

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
        dRes.InitSliderRange(RES_MIN, RES_MAX, 2);

        dRes.SetSliderListener(SetVerticies);
        //SetVerticies(dRes.GetValue());        
    }

    // Update is called once per frame
    void Update()
    {
        ShowVertices();
        MouseSupport();
    }

    #region DRAW_MESH

    void SetVerticies(float v)
    {
        TheMesh.Clear();
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
        DrawTris();
    }

    void DrawTris()
    {
        Tris = new int[(curRes - 1) * (curRes - 1) * 6];
        int temp, curRow, tNdx = 0;
        for (int i = 0; i < Vertices.Length; i++)
        {
            if(6 * tNdx < Tris.Length)
            {
                temp = i;
                curRow = 0;
                while (temp > curRes - 1)
                {
                    temp -= curRes;
                    curRow++;
                }
                if ((temp % curRes < curRes - 1) && (curRow < curRes - 1))
                {
                    Tris[6 * tNdx] = i;
                    Tris[6 * tNdx + 1] = i + curRes;
                    Tris[6 * tNdx + 2] = i + curRes + 1;

                    Tris[6 * tNdx + 3] = i;
                    Tris[6 * tNdx + 4] = i + curRes + 1;
                    Tris[6 * tNdx + 5] = i + 1;

                    tNdx++;
                }
                
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
    }

    #endregion

    public void SetVertex(Vector3 ver, int ndx)
    {
        if(ndx < Vertices.Length)
        {
            
        }
    }

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
                            //TODO: manipulate mesh
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

    
}
