using UnityEngine;

namespace TerrainGeneration
{
    public class MeshData
    {
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uvs;
        private Vector3[] bakedNormals;

        Vector3[] borderVertices;
        int[] borderTriangles;

        private int triangleIndex;
        private int borderTriangleIndex;

        private bool useFlatShading;

        public MeshData(int verticesPerLine, bool useFlatShading)
        {
            this.useFlatShading = useFlatShading;

            vertices = new Vector3[verticesPerLine * verticesPerLine];
            triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
            uvs = new Vector2[verticesPerLine * verticesPerLine];

            borderVertices = new Vector3[verticesPerLine * 4 + 4];
            borderTriangles = new int[verticesPerLine * 24];
        }

        public void AddVertex(Vector3 vertex, Vector2 uv, int index)
        {
            if (index < 0)
            {
                borderVertices[-index - 1] = vertex;
            }
            else
            {
                vertices[index] = vertex;
                uvs[index] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                borderTriangles[borderTriangleIndex++] = a;
                borderTriangles[borderTriangleIndex++] = b;
                borderTriangles[borderTriangleIndex++] = c;
            }
            else
            {
                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = c;
            }

        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            if (useFlatShading)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.normals = bakedNormals;
            }
            return mesh;
        }



        public Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[vertices.Length];
            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                int triangleIndex = i * 3;
                int vertexIndexA = triangles[triangleIndex];
                int vertexIndexB = triangles[triangleIndex + 1];
                int vertexIndexC = triangles[triangleIndex + 2];

                Vector3 triangleNormal = GetNormalFromVertexIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            int borderTriangleCount = borderTriangles.Length / 3;
            for (int i = 0; i < borderTriangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = borderTriangles[normalTriangleIndex];
                int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
                int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = GetNormalFromVertexIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0)
                {
                    vertexNormals[vertexIndexA] += triangleNormal;
                }
                if (vertexIndexB >= 0)
                {
                    vertexNormals[vertexIndexB] += triangleNormal;
                }
                if (vertexIndexC >= 0)
                {
                    vertexNormals[vertexIndexC] += triangleNormal;
                }
            }

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        public void ProcessMesh()
        {
            if (useFlatShading)
            {
                FlatShading();
            }
            else
            {
                BakeNormals();
            }
        }

        private void BakeNormals()
        {
            bakedNormals = CalculateNormals();
        }

        private Vector3 GetNormalFromVertexIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
            Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
            Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

            Vector3 ab = pointB - pointA;
            Vector3 ac = pointC - pointA;
            return Vector3.Cross(ab, ac).normalized;
        }

        private void FlatShading()
        {
            int triangleCount = triangles.Length;
            Vector3[] flatShadedVertices = new Vector3[triangleCount];
            Vector2[] flatShadedUVs = new Vector2[triangleCount];

            for (int i = 0; i < triangleCount; i++)
            {
                flatShadedVertices[i] = vertices[triangles[i]];
                flatShadedUVs[i] = uvs[triangles[i]];
                triangles[i] = i;
            }

            vertices = flatShadedVertices;
            uvs = flatShadedUVs;
        }
    }
}