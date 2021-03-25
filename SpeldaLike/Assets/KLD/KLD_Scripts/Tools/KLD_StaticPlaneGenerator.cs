using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public static class KLD_StaticPlaneGenerator
{

    public static Vector2Int planeSquares = new Vector2Int(30, 30);
    public static Vector2 squareSize = new Vector2(1f, 1f);

    public static float noiseStrengh = 1f;
    public static float noiseScale = 5f;
    public static float seaLevel = 5f;

    public static Material material;

    public static MeshFilter meshNormalsToDraw;

    public static void CreatePlane(bool _flatPlane)
    {

        GameObject curGO = new GameObject("newPolyplane");
        MeshFilter meshFilter = curGO.AddComponent<MeshFilter>();
        curGO.AddComponent<MeshRenderer>().material = material;

        Mesh mesh;
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.Clear();

        mesh.vertices = GenerateVertices(_flatPlane);
        mesh.triangles = GenerateTriangles();

        mesh.RecalculateNormals();
        //mesh.Optimize();
    }

    public static void CountVertice(MeshFilter _meshFilter)
    {
        Debug.Log(_meshFilter.gameObject.name + "has a mesh that has " + _meshFilter.mesh.vertices.GetLength(0) + " vertices");
    }

    public static void Flatten(MeshFilter _meshFilter)
    {
        Vector3[] verticesInst = _meshFilter.mesh.vertices;

        for (int i = 0; i < verticesInst.GetLength(0); i++)
        {
            verticesInst[i] = new Vector3(verticesInst[i].x, 0f, verticesInst[i].z);
            //if (i != 2 && i != 5 && i != 103 && i != 104)
            //{
            //}
        }

        _meshFilter.mesh.vertices = verticesInst;
        _meshFilter.mesh.RecalculateNormals();
    }

    public static void RecalculateNormals(MeshFilter _meshFilter)
    {
        _meshFilter.mesh.RecalculateNormals();

    }

    static Vector3[] GenerateVertices(bool _flatPlane)
    {
        Vector3[] verticesInst = new Vector3[planeSquares.x * planeSquares.y * 6];

        Vector2 randomOffset = new Vector2(Random.value * 10000f, Random.value * 10000f);

        for (int y = 0; y < planeSquares.y; y++)
        {
            for (int x = 0; x < planeSquares.x; x++)
            {
                int cornerIndex = ((y * planeSquares.x) + x) * 6;
                Vector3 cornerPosition = new Vector3(x * squareSize.x, 0f, y * squareSize.y);

                //verticesInst[cornerIndex] = cornerPosition;
                //verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y;
                //verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;
                //verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x;
                //verticesInst[cornerIndex + 4] = cornerPosition;
                //verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;

                //verticesInst[cornerIndex] = cornerPosition + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 4] = cornerPosition + Vector3.up * Random.value;
                //verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x + Vector3.up * Random.value;

                verticesInst[cornerIndex] = cornerPosition;
                verticesInst[cornerIndex + 1] = cornerPosition + Vector3.forward * squareSize.y;
                verticesInst[cornerIndex + 2] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;
                verticesInst[cornerIndex + 3] = cornerPosition + Vector3.right * squareSize.x;
                verticesInst[cornerIndex + 4] = cornerPosition;
                verticesInst[cornerIndex + 5] = cornerPosition + Vector3.forward * squareSize.y + Vector3.right * squareSize.x;

                if (!_flatPlane)
                {

                    verticesInst[cornerIndex + 0] = verticesInst[cornerIndex + 0] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 0].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 0].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                    verticesInst[cornerIndex + 1] = verticesInst[cornerIndex + 1] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 1].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 1].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                    verticesInst[cornerIndex + 2] = verticesInst[cornerIndex + 2] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 2].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 2].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                    verticesInst[cornerIndex + 3] = verticesInst[cornerIndex + 3] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 3].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 3].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                    verticesInst[cornerIndex + 4] = verticesInst[cornerIndex + 4] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 4].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 4].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                    verticesInst[cornerIndex + 5] = verticesInst[cornerIndex + 5] + Vector3.up * Mathf.PerlinNoise(verticesInst[cornerIndex + 5].x / ((float)squareSize.x * noiseScale) + randomOffset.x, verticesInst[cornerIndex + 5].z / ((float)squareSize.y * noiseScale) + randomOffset.y) * noiseStrengh;
                }

                verticesInst[cornerIndex].y += seaLevel;
                verticesInst[cornerIndex + 1].y += seaLevel;
                verticesInst[cornerIndex + 2].y += seaLevel;
                verticesInst[cornerIndex + 3].y += seaLevel;
                verticesInst[cornerIndex + 4].y += seaLevel;
                verticesInst[cornerIndex + 5].y += seaLevel;


            }
        }
        Debug.Log("generated " + verticesInst.GetLength(0) + " vertices");
        return verticesInst;
    }

    static int[] GenerateTriangles()
    {
        int[] trianglesInst = new int[planeSquares.x * planeSquares.y * 6];

        for (int y = 0; y < planeSquares.y; y++)
        {
            for (int x = 0; x < planeSquares.x; x++)
            {
                int verticeCornerIndex = ((y * planeSquares.x) + x) * 6;

                int triangleIntIndex = ((y * planeSquares.x) + x) * 6;

                trianglesInst[triangleIntIndex] = verticeCornerIndex;
                trianglesInst[triangleIntIndex + 1] = verticeCornerIndex + 1;
                trianglesInst[triangleIntIndex + 2] = verticeCornerIndex + 2;

                trianglesInst[triangleIntIndex + 3] = verticeCornerIndex + 4;
                trianglesInst[triangleIntIndex + 4] = verticeCornerIndex + 5;
                trianglesInst[triangleIntIndex + 5] = verticeCornerIndex + 3;
            }

        }

        return trianglesInst;
    }

}
