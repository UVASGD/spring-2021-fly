using UnityEngine;
using System.Collections;

public class MapPreview : MonoBehaviour
{

    public TerrainGenerator terrainGenerator;

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode { NoiseMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public Material terrainMaterial;



    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLOD;
    public bool autoUpdate;




    public void DrawMapInEditor()
    {
        terrainGenerator.textureSettings.ApplyToMaterial(terrainMaterial);
        terrainGenerator.textureSettings.UpdateMeshHeights(terrainMaterial, terrainGenerator.heightMapSettings.minHeight, terrainGenerator.heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(terrainGenerator.meshSettings.numVertsPerLine, terrainGenerator.meshSettings.numVertsPerLine, terrainGenerator.heightMapSettings, Vector2.zero);

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, terrainGenerator.meshSettings, editorPreviewLOD));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(terrainGenerator.meshSettings.numVertsPerLine), 0, 1)));
        }
    }





    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }



    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        terrainGenerator.textureSettings.ApplyToMaterial(terrainMaterial);
    }

    void OnValidate()
    {

        if (terrainGenerator.meshSettings != null)
        {
            terrainGenerator.meshSettings.OnValuesUpdated -= OnValuesUpdated;
            terrainGenerator.meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (terrainGenerator.heightMapSettings != null)
        {
            terrainGenerator.heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            terrainGenerator.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (terrainGenerator.textureSettings != null)
        {
            terrainGenerator.textureSettings.OnValuesUpdated -= OnTextureValuesUpdated;
            terrainGenerator.textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }

}
