using UnityEngine;
using System.Collections.Generic;

public class TerrainChunk {
	
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	 
	GameObject meshObject;
	Vector2 sampleCentre;
	Bounds bounds;

    List<GameObject> thingsToGenerate = new List<GameObject>();

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	Transform viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, List<ThingToGenerate> things) {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        foreach (ThingToGenerate thing in things)
        {
            int numToGen = (int)(thing.minPerChunk + (Random.value * (thing.maxPerChunk + 1 - thing.minPerChunk)));

            for (int i = 0; i < numToGen; i++)
            {
                Vector3 positionToGen = new Vector3(position.x + Random.Range(-1 * bounds.extents[0], bounds.extents[0]), 0, position.y + Random.Range(-1 * bounds.extents[0], bounds.extents[0]));
                this.thingsToGenerate.Add(GameObject.Instantiate(thing.thingGenerating, positionToGen, Quaternion.identity, parent));
            }
        }


        meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x,0,position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++) {
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex) {
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;

	}

	public void Load() {
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
	}



	void OnHeightMapReceived(object heightMapObject) {
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk ();
	}

	Vector2 viewerPosition {
		get {
			return new Vector2 (viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() {
		if (heightMapReceived) {
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));

			bool wasVisible = IsVisible ();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible) {
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++) {
					if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) {
						lodIndex = i + 1;
					} else {
						break;
					}
				}

				if (lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshes [lodIndex];
					if (lodMesh.hasMesh) {
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh (heightMap, meshSettings);
					}
				}


			}

			if (wasVisible != visible) {
				
				SetVisible (visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged (this, visible);
				}
			}
		}
	}

	public void UpdateCollisionMesh() {
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance (viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes [colliderLODIndex].hasRequestedMesh) {
					lodMeshes [colliderLODIndex].RequestMesh (heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes [colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes [colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
        foreach (GameObject thingToGenerate in thingsToGenerate) {
            thingToGenerate.SetActive(visible);
        }
    }

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}

class LODMesh {

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
		mesh = ((MeshData)meshDataObject).CreateMesh ();
		hasMesh = true;

		updateCallback ();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}