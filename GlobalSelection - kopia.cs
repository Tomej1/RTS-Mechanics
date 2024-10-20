using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSelection : MonoBehaviour
{
    SelectedDictionary selectedTable;
    RaycastHit hit;

    bool dragSelect;

    //Collider variables
    //=======================================================//

    MeshCollider selectionBox;
    Mesh selectionMesh;

    Vector3 p1;
    Vector3 p2;

    //the corners of our 2d selection box
    Vector2[] corners;

    //the vertices of our meshcollider
    Vector3[] verts;
    Vector3[] vecs;

    // Start is called before the first frame update
    void Start()
    {

        selectedTable = GetComponent<SelectedDictionary>();
        dragSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        //1. when left mouse button clicked (but not released)
        if (Input.GetMouseButtonDown(0))
        {
            p1 = Input.mousePosition;
        }

        //2. while left mouse button held
        if (Input.GetMouseButton(0))
        {
            if ((p1 - Input.mousePosition).magnitude > 40)
            {
                dragSelect = true;
            }
        }

        //3. when mouse button comes up
        if (Input.GetMouseButtonUp(0))
        {
            if (dragSelect == false) //single select
            {
                Ray ray = Camera.main.ScreenPointToRay(p1);

                if (Physics.Raycast(ray, out hit, 50000.0f))
                {
                    if (Input.GetKey(KeyCode.LeftShift)) //inclusive select
                    {
                        selectedTable.AddSelected(hit.transform.gameObject);
                    }
                    else //exclusive selected
                    {
                        selectedTable.DeselectAll();
                        selectedTable.AddSelected(hit.transform.gameObject);
                    }
                }
                else //if we didnt hit something
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //do nothing
                    }
                    else
                    {
                        selectedTable.DeselectAll();
                    }
                }
            }
            else //marquee select
            {
                verts = new Vector3[4];
                vecs = new Vector3[4];
                int i = 0;
                p2 = Input.mousePosition;
                corners = GetBoundingBox(p1, p2);

                foreach (Vector2 corner in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
                    {
                        verts[i] = new Vector3(hit.point.x, 0, hit.point.z);
                        vecs[i] = ray.origin - hit.point;
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 1.0f);
                    }
                    i++;
                }

                //generate the mesh
                selectionMesh = GenerateSelectionMesh(verts, vecs);

                selectionBox = gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    selectedTable.DeselectAll();
                }

                Destroy(selectionBox, 0.02f);

            }//end marquee select

            dragSelect = false;
        }

    }

    private void OnGUI()
    {
        if (dragSelect == true)
        {
            var rect = Utils.GetScreenRect(p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        
            // This decides what units are withing the rectangle box

            Vector2 newP1;
            Vector2 newP2;
            Vector2 newP3;
            Vector2 newP4;

            // p1 is is to the left of p2
            if (p1.x < p2.x)
            {
                // If p1 is above p2
                if (p1.y > p2.y)
                {
                    newP1 = p1;
                    newP2 = new Vector2(p2.x, p1.y);
                    newP3 = new Vector2(p1.x, p2.y);
                    newP4 = p2;
                }
                // If p1 is below p2
                else
                {
                    newP1 = new Vector2(p1.x, p2.y);
                    newP2 = p2;
                    newP3 = p1;
                    newP4 = new Vector2(p2.x, p1.y);
                }
            }
            // If p1 is to the right of p2
            else
            {
                // If p1 is aboive p2
                if (p1.y > p2.y)
                {
                    newP1 = new Vector2(p2.x, p1.y);
                    newP2 = p1;
                    newP3 = p2;
                    newP4 = new Vector2(p1.x, p2.y);
                }
                // If p1 is below p2
                else
                {
                    newP1 = p2;
                    newP2 = new Vector2(p1.x, p2.y);
                    newP3 = new Vector2(p2.x, p1.y);
                    newP4 = p1;
                }
            }

            Vector2[] corners = { newP1, newP2, newP3, newP4 };
            return corners;

    }

    //generate a mesh from the 4 bottom points
   

    Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        selectedTable.AddSelected(other.gameObject);
    }

}
