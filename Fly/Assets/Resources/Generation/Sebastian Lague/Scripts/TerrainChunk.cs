using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainChunk
    {
        private ProceduralTerrainManager proceduralTerrainManager;

        private GameObject meshObject;
        private Bounds bounds;
        public Vector2Int coordinate { get; private set; }
        public Vector3 position { get; private set; }
        public bool visible { get; private set; }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        public LODInfo[] detailLevels;
        public LODMesh[] lodMeshes;
        
        public MapData mapData;
        public bool mapDataReceived;
        public int previousLODIndex;

        public TerrainChunk(Vector2Int coord, int size, LODInfo[] detailLevels, Transform parent, ProceduralTerrainManager proceduralTerrainManager, Material material)
        {
            this.proceduralTerrainManager = proceduralTerrainManager;
            this.detailLevels = detailLevels;


            int detailLevelCount = this.detailLevels.Length;
            lodMeshes = new LODMesh[detailLevelCount];
            previousLODIndex = -1;

            coordinate = coord;
            position = new Vector3(coordinate.x, 0f, coordinate.y) * size;
            bounds = new Bounds(position, Vector2.one * size);

            for (int i = 0; i < detailLevelCount; i++)
            {
                lodMeshes[i] = new LODMesh(proceduralTerrainManager.mapGenerator, coordinate, this.detailLevels[i].lod, UpdateTerrainChunk);
            }

            meshObject = new GameObject($"TerrainChunk({coordinate.x}, {coordinate.y})");
            meshObject.transform.position = position * proceduralTerrainManager.mapGenerator.terrainData.uniformScale;
            meshObject.transform.SetParent(parent);
            meshObject.transform.localScale = Vector3.one * proceduralTerrainManager.mapGenerator.terrainData.uniformScale;

            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshRenderer.material = material;

            SetVisible(false);

            proceduralTerrainManager.mapGenerator.RequestMapData(new Vector2(position.x, position.z), OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (!mapDataReceived) return;

            float viewerDistanceFromBounds = GetDistanceFromViewer();
            SetVisible(viewerDistanceFromBounds <= proceduralTerrainManager.maxViewDistance);

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromBounds > detailLevels[i].visibleDistanceThreshold)
                    {
                        lodIndex = i + 1;
                    } 
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;

                        if (lodIndex == 0)
                        {
                            if (!lodMesh.hasCollider)
                            {
                                meshCollider.sharedMesh = lodMesh.mesh;
                                lodMesh.hasCollider = true;
                            }
                            meshCollider.enabled = true;
                        }
                        else
                        {
                            meshCollider.enabled = false;
                        }
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData);
                    }
                }

                proceduralTerrainManager.terrainChunksVisibleLastUpdate.Add(this);
            }
        }

        public void SetVisible(bool visible)
        {
            this.visible = visible;
            meshObject.SetActive(visible);
        }

        public float GetDistanceFromViewer()
        {
            return Mathf.Sqrt(bounds.SqrDistance(proceduralTerrainManager.viewerPosition));
        }
    }
}