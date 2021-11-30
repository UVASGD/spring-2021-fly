using UnityEngine;
using System;

namespace TerrainGeneration
{
    [Serializable]
    public class LODMesh
    {
        public MapGenerator mapGenerator;
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public bool hasCollider;
        private int lod;
        private Action updateCallback;
        private Vector2Int coordinate;

        public LODMesh(MapGenerator mapGenerator, Vector2Int coordinate, int lod, Action updateCallback)
        {
            hasRequestedMesh = false;
            hasMesh = false;
            hasCollider = false;
            this.mapGenerator = mapGenerator;
            this.coordinate = coordinate;
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            mesh.name = $"TerrainChunk({coordinate.x}, {coordinate.y})";
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
            hasRequestedMesh = true;
        }
    }
}