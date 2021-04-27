using UnityEngine;
using System.Collections.Generic;

public class TerrainChunk
{

	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;

	GameObject meshObject;
	Vector2 sampleCentre;
	Bounds bounds;

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

	List<GameObject> gameObjectsInChunk;
	List<TerrainObject> terrainObjects;

	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, List<TerrainObject> terrainObjects)
	{
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;
		this.terrainObjects = terrainObjects;

		gameObjectsInChunk = new List<GameObject>();

		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize;
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();

		meshObject.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		meshObject.GetComponent<Rigidbody>().useGravity = false;
		meshObject.GetComponent<Rigidbody>().isKinematic = true;
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x, 0, position.y);
		meshObject.transform.parent = parent;

		SetVisible(false);

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++)
		{
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex)
			{
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

		// Instantiate terrain objects
		float size = meshSettings.meshWorldSize / 2f;
		foreach (TerrainObject obj in terrainObjects)
		{
			int numToGen = (int)(obj.minPerChunk + (Random.value * (obj.maxPerChunk + 1 - obj.minPerChunk)));

			for (int i = 0; i < numToGen; i++)
			{
				Vector3 positionToGen = new Vector3(Random.Range(-size, size), 0, Random.Range(-size, size));

				// Skip generation if too close to existing object
				Collider[] colliders = Physics.OverlapSphere(positionToGen, 5f);
				foreach (var collider in colliders)
				{
					if (!collider.gameObject.name.Contains("Chunk"))
					{
						continue;
					}
				}

				GameObject instance = GameObject.Instantiate(obj.prefab, meshObject.transform);
				instance.SetActive(true);
				instance.transform.localPosition = positionToGen;
				instance.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
				gameObjectsInChunk.Add(instance);

				//Debug.Log($"Created object at {instance.transform.localPosition}");
			}
		}

		// Snap all objects to the chunk surface
		foreach (var obj in gameObjectsInChunk)
		{
			if (obj.GetComponent<PutObjectOnTerrain>() != null)
			{
				obj.GetComponent<PutObjectOnTerrain>().SnapToTerrain(meshSettings, heightMapSettings);
			}
		}
	}

	public void Load()
	{
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
	}

	public void SetGameObjectsInChunk(List<GameObject> chunkObjects)
	{
		gameObjectsInChunk = chunkObjects;
	}

	void OnHeightMapReceived(object heightMapObject)
	{
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk();
	}

	Vector2 viewerPosition
	{
		get
		{
			if (viewer != null)
			{
				return new Vector2(viewer.position.x, viewer.position.z);
			}
			else
			{
				return Vector2.zero;
			}
		}
	}


	public void UpdateTerrainChunk()
	{
		if (heightMapReceived)
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

			bool wasVisible = IsVisible();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible)
			{
				int lodIndex = 0;
				for (int i = 0; i < detailLevels.Length - 1; i++)
				{
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
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
					}
					else if (!lodMesh.hasRequestedMesh)
					{
						lodMesh.RequestMesh(heightMap, meshSettings);
					}
				}


			}

			if (wasVisible != visible)
			{

				SetVisible(visible);
				if (onVisibilityChanged != null)
				{
					onVisibilityChanged(this, visible);
				}
			}
		}
	}

	public void UpdateCollisionMesh()
	{
		if (!hasSetCollider)
		{
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
			{
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
				{
					lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
			{
				if (lodMeshes[colliderLODIndex].hasMesh)
				{
					meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

	public void SetVisible(bool visible)
	{
		meshObject.SetActive(visible);
		if (gameObjectsInChunk != null)
		{
			foreach (GameObject go in gameObjectsInChunk)
			{
				if (go != null)
				{
					go.SetActive(visible);
				}
			}
		}

	}

	public bool IsVisible()
	{
		return meshObject.activeSelf;
	}

	public Bounds GetBounds()
	{
		return bounds;
	}

}

class LODMesh
{

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod)
	{
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject)
	{
		mesh = ((MeshData)meshDataObject).CreateMesh();
		hasMesh = true;

		updateCallback();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
	{
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}