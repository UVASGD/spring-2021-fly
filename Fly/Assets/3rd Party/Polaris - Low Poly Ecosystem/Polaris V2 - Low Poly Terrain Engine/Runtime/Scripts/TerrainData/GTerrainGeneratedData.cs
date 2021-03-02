using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    //[CreateAssetMenu(menuName = "Griffin/Generated Data")]
    [PreferBinarySerialization]
    public class GTerrainGeneratedData : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        [HideInInspector]
        private GTerrainData terrainData;
        public GTerrainData TerrainData
        {
            get
            {
                return terrainData;
            }
            internal set
            {
                terrainData = value;
            }
        }

        private Dictionary<string, Mesh> generatedMeshes;
        private Dictionary<string, Mesh> GeneratedMeshes
        {
            get
            {
                if (generatedMeshes == null)
                    generatedMeshes = new Dictionary<string, Mesh>();
                return generatedMeshes;
            }
        }

        [SerializeField]
        [HideInInspector]
        private List<string> generatedMeshesKeys;

        [SerializeField]
        [HideInInspector]
        private List<Mesh> generatedMeshesValues;

        private void Reset()
        {
            generatedMeshes = new Dictionary<string, Mesh>();
            generatedMeshesKeys = new List<string>();
            generatedMeshesValues = new List<Mesh>();
        }

        public void OnBeforeSerialize()
        {
            generatedMeshesKeys.Clear();
            generatedMeshesValues.Clear();
            foreach (string k in GeneratedMeshes.Keys)
            {
                if (GeneratedMeshes[k] != null)
                {
                    generatedMeshesKeys.Add(k);
                    generatedMeshesValues.Add(GeneratedMeshes[k]);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (generatedMeshesKeys == null || generatedMeshesValues == null)
                return;
            GeneratedMeshes.Clear();
            for (int i = 0; i < generatedMeshesKeys.Count; ++i)
            {
                string k = generatedMeshesKeys[i];
                Mesh m = generatedMeshesValues[i];
                if (!string.IsNullOrEmpty(k) && m != null)
                {
                    GeneratedMeshes[k] = m;
                }
            }
        }

        public void SetMesh(string key, Mesh mesh)
        {
            if (GeneratedMeshes.ContainsKey(key))
            {
                Mesh oldMesh = GeneratedMeshes[key];
                if (oldMesh != null)
                {
                    GUtilities.DestroyObject(oldMesh);
                }
                GeneratedMeshes.Remove(key);
            }
            GCommon.TryAddObjectToAsset(mesh, this);
            GeneratedMeshes.Add(key, mesh);
            GCommon.SetDirty(this);
        }

        public Mesh GetMesh(string key)
        {
            if (GeneratedMeshes.ContainsKey(key))
                return GeneratedMeshes[key];
            else
                return null;
        }

        public void DeleteMesh(string key)
        {
            if (GeneratedMeshes.ContainsKey(key))
            {
                Mesh m = GeneratedMeshes[key];
                if (m != null)
                {
                    GUtilities.DestroyObject(m);
                }
                GeneratedMeshes.Remove(key);
                GCommon.SetDirty(this);
            }
        }

        public List<string> GetKeys()
        {
            return new List<string>(GeneratedMeshes.Keys);
        }

        public void Internal_DeleteMeshIf(System.Predicate<string> condition)
        {
            List<string> keys = GetKeys();
            for (int i = 0; i < keys.Count; ++i)
            {
                if (condition(keys[i]))
                {
                    DeleteMesh(keys[i]);
                }
            }
        }
    }
}
