using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TerrainGeneration
{
    [RequireComponent(typeof(MapGenerator))]
    public class ProceduralTerrainManager : MonoBehaviour
    {
        const float viewerMoveThresholdForChunkUpdate = 25f;
        const float viewerMoveThresholdForChunkUpdatedSquared = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

        public LODInfo[] detailLevels;
        public float maxViewDistance { get; private set; }
        public Vector3 viewerPosition { get; private set; }
        private Vector3 viewerPositionAtLastUpdate;

        public MapGenerator mapGenerator { get; private set; }

        public Transform viewer;
        public Material mapMaterial;
        
        public int chunkSize { get; private set; }
        public int chunksVisibleRadius { get; private set; }
        public Transform chunkParent;

        private Dictionary<Vector2Int, TerrainChunk> terrainChunkMap;
        [HideInInspector]
        public List<TerrainChunk> terrainChunksVisibleLastUpdate;

        private void Start()
        {
            mapGenerator = GetComponent<MapGenerator>();

            maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
            chunkSize = MapGenerator.mapChunkSize - 1;
            chunksVisibleRadius = Mathf.RoundToInt(maxViewDistance / chunkSize);
            terrainChunkMap = new Dictionary<Vector2Int, TerrainChunk>();
            terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

            if (viewer == null)
            {
                viewer = transform; // Static player at origin
            }

            UpdateVisibleChunks();
        }

        private void Update()
        {
            viewerPosition = viewer.position / mapGenerator.terrainData.uniformScale;

            if (Vector3.SqrMagnitude(viewerPosition - viewerPositionAtLastUpdate) > viewerMoveThresholdForChunkUpdatedSquared)
            {
                UpdateVisibleChunks();
                viewerPositionAtLastUpdate = viewerPosition;
            }
        }

        void UpdateVisibleChunks()
        {
            foreach (var chunk in terrainChunksVisibleLastUpdate)
            {
                chunk.SetVisible(false);
            }
            terrainChunksVisibleLastUpdate.Clear();

            Vector2Int currentChunkCoords = WorldToChunkSpace(viewerPosition);

            for (int yOffset = -chunksVisibleRadius; yOffset <= chunksVisibleRadius; yOffset++)
            {
                for (int xOffset = -chunksVisibleRadius; xOffset <= chunksVisibleRadius; xOffset++)
                {
                    Vector2Int viewedChunkCoord = new Vector2Int(currentChunkCoords.x + xOffset, currentChunkCoords.y + yOffset);

                    if (terrainChunkMap.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkMap[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        terrainChunkMap.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, chunkParent, this, mapMaterial));
                    }
                }
            }
        }

        public Vector2Int WorldToChunkSpace(Vector3 worldSpace)
        {
            Vector2Int chunkSpace = new Vector2Int(Mathf.RoundToInt(worldSpace.x / chunkSize), Mathf.RoundToInt(worldSpace.z / chunkSize));
            return chunkSpace;
        }

        public Vector3 ChunkToWorldSpace(Vector2Int chunkSpace)
        {
            Vector3 worldSpace = new Vector3(chunkSpace.x * chunkSize, 0f, chunkSpace.y * chunkSize);
            return worldSpace;
        }
    }
}