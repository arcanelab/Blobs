using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob: MonoBehaviour
{
    new public Renderer renderer;
    public MeshFilter meshFilter;
    public Material gridMaterial;
    public Material blobMaterial;
    private Transform[] points = new Transform[3];
    private float halfpi = Mathf.PI / 2f;
    private float clock = 0;
    private float amplitude = 5;
    private const int dim = 15;
    private Transform[,,] gridTransforms = new Transform[dim, dim, dim];
    private float[,,] grid = new float[dim, dim, dim];

    void OnEnable()
    {
        if(meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if(renderer == null)
            renderer = GetComponent<Renderer>();

        for(int i = 0; i < points.Length; i++)
        {
            points[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            points[i].gameObject.GetComponent<Renderer>().sharedMaterial = blobMaterial;
            points[i].gameObject.GetComponent<Renderer>().enabled = false;
        }

        InitializeGrid();
    }

    private void OnDisable()
    {
        Destroy(GameObject.Find("Grid"));
        for(int i = 0; i < points.Length; i++)
        {
            Destroy(points[i].gameObject);
        }
    }

    void InitializeGrid()
    {
        GameObject parent = new GameObject();
        parent.name = "Grid";

        for(int z = 0; z < dim; z++)
        {
            for(int y = 0; y < dim; y++)
            {
                for(int x = 0; x < dim; x++)
                {
                    gridTransforms[x, y, z] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                    gridTransforms[x, y, z].position = new Vector3Int(x - (int)amplitude, y - (int)amplitude, z - (int)amplitude);
                    gridTransforms[x, y, z].parent = parent.transform;
                    gridTransforms[x, y, z].gameObject.GetComponent<Renderer>().material = gridMaterial;
                }
            }
        }
    }

    void Update()
    {
        MovePoints();
        UpdateDistanceField();
    }

    void MovePoints()
    {
        clock += Time.deltaTime;
        float t = clock * 20;

        Vector3 v = new Vector3();
        float[] a = { 3, 2, 4 };
        float[] b = { 2, 3, 4 };

        for(int i = 0; i < points.Length; i++)
        {
            v.x = amplitude * Mathf.Sin(Mathf.Deg2Rad * a[i] * t + halfpi);
            v.y = amplitude * Mathf.Cos(Mathf.Deg2Rad * b[i] * t);
            v.z = amplitude * Mathf.Sin(Mathf.Deg2Rad * a[i] * t);

            points[i].position = v;
        }
    }

    void UpdateDistanceField()
    {
        Vector3 p = new Vector3();
        Vector3 offset = new Vector3(amplitude, amplitude, amplitude);
        for(int z = 0; z < dim; z++)
        {
            for(int y = 0; y < dim; y++)
            {
                for(int x = 0; x < dim; x++)
                {
                    grid[x, y, z] = 0;
                    float d = 0;
                    for(int i = 0; i < points.Length; i++)
                    {
                        p.Set(x, y, z);
                        d = Vector3.SqrMagnitude((points[i].position + offset) - p);
                        //d = Vector3.Distance((points[i].position + offset), p);
                        Mathf.Clamp(d, 0.1f, 10f);
                        grid[x, y, z] += 1 * (1 / (1 + 1.2f * d));
                    }

                    //gridTransforms[x, y, z].localScale = Vector3.ClampMagnitude(Vector3.one * ((grid[x, y, z] > 0.2f) ? 1 : 0.01f), 3);
                    gridTransforms[x, y, z].localScale = Vector3.ClampMagnitude(Vector3.one * grid[x, y, z], 2);
                }
            }
        }
    }
}
