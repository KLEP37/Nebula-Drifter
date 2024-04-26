using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisualObject : MonoBehaviour
{
    public SpaceObject spaceObject;

    public float radius = 1f;

    MeshFilter meshFilter;
    Mesh mesh;

    public List<Vector3> vertices = new List<Vector3>();
    public List<Triangle> triangles = new List<Triangle>();

    void Awake()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        this.AddComponent<MeshRenderer>();
    }
    public void Initiate()
    {
        mesh.Clear();
        CreateIcosahedronVertices(radius);
        CreateIcosahedronTriangles();
        UpdateMesh();
        StartCoroutine(UpdateTriangles(2));
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].trash)
            {
                triangles.RemoveAt(i);
            }
        }

        mesh.vertices = vertices.ToArray();
        var triangleIndexes = new List<int>();
        foreach (var triangle in triangles)
        {
            for (int i = 0; i < 3; i++)
            {
                triangleIndexes.Add(triangle.verticeIndex[i]);
            }
        }
        mesh.triangles = triangleIndexes.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public Vector3 GetTerrainOffset(Vector3 vertice, int tier)
    {
        /* !!! FCKING HORRIBLE BUG => Load edges are see through
        if (triangles.Where(x => x.vertice.Contains(vertice)).Count() <= 3)
        {
            continue;
        }
        */
        float z = (Mathf.PerlinNoise(vertice.x, vertice.y) + Mathf.PerlinNoise(vertice.x, vertice.z) + Mathf.PerlinNoise(vertice.y, vertice.z)) / 3 - 0.5f;
        z /= Mathf.Pow(2, tier);
        Debug.Log(z);
        var x = (1 + z) * radius / vertice.magnitude;
        vertice *= x;
        return vertice;
    }

    void CreateIcosahedronVertices(float radius)
    {
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;

        vertices.Add(new Vector3(-1, phi, 0).normalized * radius);
        vertices.Add(new Vector3(1, phi, 0).normalized * radius);
        vertices.Add(new Vector3(-1, -phi, 0).normalized * radius);
        vertices.Add(new Vector3(1, -phi, 0).normalized * radius);

        vertices.Add(new Vector3(0, -1, phi).normalized * radius);
        vertices.Add(new Vector3(0, 1, phi).normalized * radius);
        vertices.Add(new Vector3(0, -1, -phi).normalized * radius);
        vertices.Add(new Vector3(0, 1, -phi).normalized * radius);

        vertices.Add(new Vector3(phi, 0, -1).normalized * radius);
        vertices.Add(new Vector3(phi, 0, 1).normalized * radius);
        vertices.Add(new Vector3(-phi, 0, -1).normalized * radius);
        vertices.Add(new Vector3(-phi, 0, 1).normalized * radius);
    }
    void CreateIcosahedronTriangles()
    {
        int[] triangles = new int[]
        {
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,

            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,

            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,

            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        for (int i = 0; i < 20; i++)
        {
            this.triangles.Add(new Triangle(new int[] { triangles[i * 3], triangles[i * 3 + 1], triangles[i * 3 + 2] }, this, 0));
        }
    }

    IEnumerator UpdateTriangles(float frequency)
    {
        yield return new WaitForSeconds(frequency);
        int changes = 0;
        for (int i = 0; i < triangles.Count; i++)
        {
            if (Vector3.Distance(triangles[i].midpoint * transform.localScale.x + transform.position, spaceObject.starSystem.user.position) < transform.localScale.x * 10 / Mathf.Pow(1.5f, triangles[i].tier))
            {
                triangles[i].GenerateSubTriangles();
                changes++;
            }
        }
        if (changes > 0)
        {
            UpdateMesh();
        }
        StartCoroutine(UpdateTriangles(frequency));
    }
}

public class Triangle
{
    VisualObject current;
    public Vector3[] vertice;
    public int[] verticeIndex;
    public bool trash;
    public int tier;
    public Vector3 midpoint;

    public Triangle(Vector3[] vertice, VisualObject current, int tier)
    {
        this.vertice = vertice;
        this.current = current;
        verticeIndex = new int[vertice.Length];
        for (int i = 0; i < 3; i++)
        {
            vertice[i] = current.GetTerrainOffset(vertice[i], tier);
            if (!current.vertices.Contains(vertice[i]))
            {
                current.vertices.Add(vertice[i]);
            }
            verticeIndex[i] = current.vertices.IndexOf(vertice[i]);
        }
        midpoint = (vertice[0] + vertice[1] + vertice[2]) / 3;
        this.tier = tier;
    }
    public Triangle(int[] verticeIndex, VisualObject current, int tier)
    {
        this.verticeIndex = verticeIndex;
        this.current = current;
        vertice = new Vector3[verticeIndex.Length];
        for (int i = 0; i < 3; i++)
        {
            vertice[i] = current.vertices[verticeIndex[i]];
        }
        midpoint = (vertice[0] + vertice[1] + vertice[2]) / 3;
        this.tier = tier;
    }

    public void GenerateSubTriangles()
    {
        if (tier > 2)
        {
            return;
        }

        var A = (vertice[0] + vertice[1]) / 2;
        var B = (vertice[1] + vertice[2]) / 2;
        var C = (vertice[2] + vertice[0]) / 2;

        current.triangles.Add(new Triangle(new Vector3[] { vertice[0], A, C }, current, tier + 1));
        current.triangles.Add(new Triangle(new Vector3[] { A, vertice[1], B }, current, tier + 1));
        current.triangles.Add(new Triangle(new Vector3[] { C, B, vertice[2] }, current, tier + 1));
        current.triangles.Add(new Triangle(new Vector3[] { A, B, C }, current, tier + 1));

        trash = true;
    }
}