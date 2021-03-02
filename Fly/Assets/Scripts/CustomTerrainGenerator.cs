//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CustomTerrainGenerator : MonoBehaviour
//{
//    public Transform target;
//    [Range(1, 8)]
//    public int chunkRenderRadius = 3;
//    public Vector2Int targetPosition;
//    private Dictionary<Vector2Int, GameObject> terrainMap;
//    public List<Vector2Int> activeChunks;

//    [Range(1f, 2048f)]
//    public float unitsPerChunk = 16f;
//    [Range(1, 8)]
//    public int terrainResolution = 4;
//    public int verticesPerEdge;

//    [Range(0f, 5f)]
//    public float elevationFrequency = 1f;
//    public float elevationAmplitude = 10f;

//    [Range(1, 10)]
//    public int octaveDepth = 4;

//    [Range(1f, 10f)]
//    public float redistribution = 1f;

//    private List<Vector3> vertices;
//    private List<int> triangles;

//    // Start is called before the first frame update
//    void Start()
//    {
//        terrainMap = new Dictionary<Vector2Int, GameObject>();
//        activeChunks = new List<Vector2Int>();

//        chunkRenderRadius--;
//        //if (chunkRenderRadius % 2 == 0)
//        //{
//        //    chunkRenderRadius++;
//        //}
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        targetPosition = new Vector2Int((int)((target.position.x  - unitsPerChunk / 2f) / unitsPerChunk), (int)((target.position.y - unitsPerChunk / 2f) / unitsPerChunk));
        
//        HashSet<Vector2Int> chunksToRender = new HashSet<Vector2Int>();
//        for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
//        {
//            for (int j = -chunkRenderRadius; j < chunkRenderRadius; j++)
//            {
//                chunksToRender.Add(new Vector2Int(targetPosition.x + i, targetPosition.y + j));
//            }
//        }

//        // Create new chunks, or render old ones that are now in range
//        foreach (var chunk in chunksToRender)
//        {
//            if (!activeChunks.Contains(chunk))
//            {
//                activeChunks.Add(chunk);
//            }

//            if (terrainMap.ContainsKey(chunk))
//            {
//                if (terrainMap[chunk] == null)
//                {
//                    terrainMap[chunk] = GenerateChunk(chunk.x, chunk.y);
//                }
//                else
//                {
//                    terrainMap[chunk].SetActive(true);
//                }
//            }
//            else
//            {
//                terrainMap.Add(chunk, GenerateChunk(chunk.x, chunk.y));
//            }
//        }

//        // Stop rendering chunks no longer in range
//        foreach (var chunk in activeChunks)
//        {
//            if (!chunksToRender.Contains(chunk))
//            {
//                GameObject chunkObj = terrainMap[chunk];
//                if (chunkObj != null)
//                {
//                    chunkObj.SetActive(false);
//                }
//                activeChunks.Remove(chunk);
//            }
//        }
//    }

//    void UpdateChunk(int x, int y)
//    {

//    }

//    GameObject GenerateChunk(int x, int y)
//    {
//        GameObject obj = new GameObject($"Terrain({x},{y})");
//        Mesh mesh;
//        MeshRenderer meshRenderer;
//        MeshFilter meshFilter;
//        verticesPerEdge = (int)Mathf.Pow(2, terrainResolution) + 1;

//        meshRenderer = obj.AddComponent<MeshRenderer>();
//        meshFilter = obj.AddComponent<MeshFilter>();

//        mesh = meshFilter.mesh;
//        meshRenderer.material = new Material(Shader.Find("Standard"));
//        mesh.name = $"Terrain({x},{y})";

//        mesh.Clear();

//        vertices = GenerateVertices(x, y);
//        triangles = GenerateTriangles();

//        mesh.vertices = vertices.ToArray();
//        mesh.triangles = triangles.ToArray();
//        mesh.RecalculateNormals();
//        return obj;
//    }

//    List<Vector3> GenerateVertices(int x, int y)
//    {
//        vertices = new List<Vector3>();
//        for (int i = 0; i < verticesPerEdge; i++)
//        {
//            for (int j = 0; j < verticesPerEdge; j++)
//            {
//                float xn = (float)i / (verticesPerEdge - 1) * unitsPerChunk - unitsPerChunk / 2f + x * unitsPerChunk;
//                float zn = (float)j / (verticesPerEdge - 1) * unitsPerChunk - unitsPerChunk / 2f + y * unitsPerChunk;
//                float yn = PerlinNoise.Sample(xn, zn, elevationFrequency, elevationAmplitude, octaveDepth, redistribution);

//                Vector3 vertex = new Vector3(xn, yn, zn);
//                vertices.Add(vertex);
//            }
//        }
//        return vertices;
//    }

//    List<int> GenerateTriangles()
//    {
//        List<int> triangles = new List<int>();
//        for (int i = 0; i < vertices.Count - verticesPerEdge; i++)
//        {
//            if (i % verticesPerEdge == verticesPerEdge - 1) continue; // Skip over last row and column

//            // Must be clockwise for normals to be correctly oriented
//            triangles.Add(i);
//            triangles.Add(i + 1);
//            triangles.Add(i + verticesPerEdge);
//            triangles.Add(i + 1);
//            triangles.Add(i + verticesPerEdge + 1);
//            triangles.Add(i + verticesPerEdge);
//        }
//        return triangles;
//    }
//}
