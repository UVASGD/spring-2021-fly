using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu()]
public class FlatTextureData : UpdatableData {

	public Layer[] layers;

	float savedMinHeight;
	float savedMaxHeight;

	public void ApplyToMaterial(Material material) {
		
		material.SetInt ("layerCount", layers.Length);
		material.SetColorArray ("baseColours", layers.Select(x => x.main_color).ToArray());
		material.SetFloatArray ("baseStartHeights", layers.Select(x => x.start_height).ToArray());

		material.SetColorArray ("baseShadedColours", layers.Select(x => x.shaded_color).ToArray());
		material.SetFloatArray ("baseShadingSizes", layers.Select(x => x.self_shading_size).ToArray());
		material.SetFloatArray ("baseShadowEdges", layers.Select(x => x.shadow_edge_size).ToArray());
		material.SetFloatArray ("baselocalizedShading", layers.Select(x => x.localized_shading).ToArray());

		material.SetColorArray ("extraShadedColours", layers.Select(x => x.extra_shaded_color).ToArray());
		material.SetFloatArray ("extraShadingSizes", layers.Select(x => x.extra_self_shading_size).ToArray());
		material.SetFloatArray ("extraShadowEdges", layers.Select(x => x.extra_shadow_edge_size).ToArray());
		material.SetFloatArray ("extralocalizedShading", layers.Select(x => x.extra_localized_shading).ToArray());

		material.SetColorArray("specularColours", layers.Select(x => x.specular_color).ToArray());
		material.SetFloatArray("specularShadingSizes", layers.Select(x => x.specular_size).ToArray());
		material.SetFloatArray("specularShadowEdges", layers.Select(x => x.specular_edge_smoothness).ToArray());

		material.SetColorArray("rimColours", layers.Select(x => x.rim_color).ToArray());
		material.SetFloatArray("rimLightAligns", layers.Select(x => x.extra_self_shading_size).ToArray());
		material.SetFloatArray("rimShadingSizes", layers.Select(x => x.extra_shadow_edge_size).ToArray());
		material.SetFloatArray("rimShadowEdges", layers.Select(x => x.extra_localized_shading).ToArray());

		UpdateMeshHeights (material, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material material, float minHeight, float maxHeight) {
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		material.SetFloat ("minHeight", minHeight);
		material.SetFloat ("maxHeight", maxHeight);
	}

	[System.Serializable]
	public class Layer {
		public string name;
		public Color main_color;
		[Range(0, 1)]
		public float start_height;

		public Color shaded_color;
		[Range(0, 1)]
		public float self_shading_size;
		[Range(0,1)]
		public float shadow_edge_size;
		[Range(0,1)]
		public float localized_shading = 1;

		public Color extra_shaded_color;
		[Range(0, 1)]
		public float extra_self_shading_size;
		[Range(0,1)]
		public float extra_shadow_edge_size;
		[Range(0,1)]
		public float extra_localized_shading = 1;

		public Color specular_color;
		[Range(0, 1)]
		public float specular_size;
		[Range(0, 1)]
		public float specular_edge_smoothness;

		public Color rim_color;
		[Range(0, 1)]
		public float rim_light_align;
		[Range(0, 1)]
		public float rim_size;
		[Range(0, 1)]
		public float rim_edge_smoothenss;
	}
		
	 
}
