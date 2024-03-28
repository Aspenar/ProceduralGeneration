using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;
    [Range(0.0f, 1.0f)]
    public float amplitude1;

    [Range(0.0f, 1.0f)]
    public float amplitude2;
    
    [Range(0.0f, 1.0f)]
    public float amplitude3;

    [Range(0.0f, 1.0f)]
    public float frequency1;

    [Range(0.0f, 1.0f)]
    public float frequency2;

    [Range(0.0f, 1.0f)]
    public float frequency3;

    [Range(0.0f, 50.0f)]
    public float noiseStrength;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    void Update()
    {
        UpdateMesh();
    }



    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y =
                    amplitude1 * Mathf.PerlinNoise(x * frequency1, z * frequency1)
                    + amplitude2 * Mathf.PerlinNoise(x * frequency2, z * frequency2)
                    + amplitude3 * Mathf.PerlinNoise(x * frequency3, z * frequency3)
                        * noiseStrength;
                vertices[i] = new Vector3(x, y, z);

                if (y >  maxTerrainHeight)
                    maxTerrainHeight = y;
                if(y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
        for (int x = 0; x < xSize; x++)
        {
            triangles[tris + 0] = vert + 0;
            triangles[tris + 1] = vert + xSize + 1;
            triangles[tris + 2] = vert + 1;
            triangles[tris + 3] = vert + 1;
            triangles[tris + 4] = vert + xSize + 1;
            triangles[tris + 5] = vert + xSize + 2;

            vert++;
            tris += 6;
        }
        vert++;
          yield return new WaitForSeconds(0.01f);
        }

        colors = new Color[vertices.Length];

          for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    //  private void OnDrawGizmos()
    //  {
    //      if (vertices == null)
    //          return;

    //      for (int i = 0; i < vertices.Length; i++)
    //      {
    //          Gizmos.DrawSphere(vertices[i], .1f);
    //      }
    //  }
}
