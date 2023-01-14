#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;

public class MeshCombineWizard : ScriptableWizard
{
	public GameObject combineParent;
	public GameObject buildingRoot;
	public bool combineColliders = true;
	public bool groundLayer = false;
	public bool is32bit = true;

	[MenuItem("Performance/Mesh Combine Wizard")]
	static void CreateWizard()
	{
		var wizard = DisplayWizard<MeshCombineWizard>("Mesh Combine Wizard");

		// If there is selection, and the selection of one Scene object, auto-assign it
		var selectionObjects = Selection.objects;
		if (selectionObjects != null && selectionObjects.Length == 1)
		{
			var firstSelection = selectionObjects[0] as GameObject;
			if (firstSelection != null)
			{
				wizard.combineParent = firstSelection;
				wizard.buildingRoot = firstSelection.transform.root.gameObject;
			}
		}
	}

	void OnWizardCreate()
	{
		// Verify there is existing object root, otherwise bail.
		if (combineParent == null)
		{
			Debug.LogError("Mesh Combine Wizard: Parent of objects to combne not assigned. Operation cancelled.");
			return;
		}

		// Remember the original position of the object. 
		// For the operation to work, the position must be temporarily set to (0,0,0).
		Vector3 originalPosition = combineParent.transform.position;
		combineParent.transform.position = Vector3.zero;

		// Locals
		Dictionary<Material, List<MeshFilter>> materialToMeshFilterList = new Dictionary<Material, List<MeshFilter>>();
		List<GameObject> combinedObjects = new List<GameObject>();

		MeshFilter[] meshFilters = combineParent.GetComponentsInChildren<MeshFilter>();

		// Go through all mesh filters and establish the mapping between the materials and all mesh filters using it.
		foreach (var meshFilter in meshFilters)
		{
			var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				Debug.LogWarning("The Mesh Filter on object " + meshFilter.name + " has no Mesh Renderer component attached. Skipping.");
				continue;
			}

			var materials = meshRenderer.sharedMaterials;
			if (materials == null)
			{
				Debug.LogWarning("The Mesh Renderer on object " + meshFilter.name + " has no material assigned. Skipping.");
				continue;
			}

			// If there are multiple materials on a single mesh, cancel.
			if (materials.Length > 1)
			{
				// Rollback: return the object to original position
				combineParent.transform.position = originalPosition;
				Debug.LogError("Objects with multiple materials on the same mesh are not supported. Create multiple meshes from this object's sub-meshes in an external 3D tool and assign separate materials to each. Operation cancelled.");
				return;
			}
			var material = materials[0];

			// Add material to mesh filter mapping to dictionary
			if (materialToMeshFilterList.ContainsKey(material)) materialToMeshFilterList[material].Add(meshFilter);
			else materialToMeshFilterList.Add(material, new List<MeshFilter>() { meshFilter });
		}

		// For each material, create a new merged object, in the scene and in the assets folder.
		foreach (var entry in materialToMeshFilterList)
		{
			List<MeshFilter> meshesWithSameMaterial = entry.Value;
			// Create a convenient material name
			string materialName = entry.Key.ToString().Split(' ')[0];

			CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
			for (int i = 0; i < meshesWithSameMaterial.Count; i++)
			{
				combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
				combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
			}

			// Create a new mesh using the combined properties
			var format = is32bit ? IndexFormat.UInt32 : IndexFormat.UInt16;
			Mesh combinedMesh = new Mesh { indexFormat = format };
			combinedMesh.CombineMeshes(combine);
			combinedMesh.Optimize();

			// Generate UV2
			UnwrapParam unwrapParam = new UnwrapParam();
			UnwrapParam.SetDefaults(out unwrapParam);
			unwrapParam.hardAngle = 5;
			Unwrapping.GenerateSecondaryUVSet(combinedMesh, unwrapParam);

			// Create asset
			materialName += "_" + combinedMesh.GetInstanceID();

			string newAssetPath = "";
			if (buildingRoot != null) newAssetPath += buildingRoot.name + "_";
			newAssetPath += combineParent.name;

			AssetDatabase.CreateAsset(combinedMesh, "Assets/Meshes/CombinedMeshes/CombinedMeshes_" + newAssetPath +  ".asset");

			// Create game object
			string goName = "CombinedMeshes_" + combineParent.name;
			GameObject combinedObject = new GameObject(goName);
			var filter = combinedObject.AddComponent<MeshFilter>();
			filter.sharedMesh = combinedMesh;
			var renderer = combinedObject.AddComponent<MeshRenderer>();
			renderer.sharedMaterial = entry.Key;
			combinedObjects.Add(combinedObject);
		}

		// If there were more than one material, and thus multiple GOs created, parent them and work with result
		GameObject resultGO = null;
		if (combinedObjects.Count > 1)
		{
			resultGO = new GameObject("CombinedMeshes_" + combineParent.name);
			foreach (var combinedObject in combinedObjects) combinedObject.transform.parent = resultGO.transform;
		}
		else
		{
			resultGO = combinedObjects[0];
		}

		// Disable the original and return both to original positions
		combineParent.SetActive(false);
		combineParent.transform.position = originalPosition;

		resultGO.transform.parent = combineParent.transform.parent;
		resultGO.transform.position = originalPosition;

		// Add all box colliders from descendants
		if (combineColliders)
			AddBoxColliderFromChildren(resultGO, combineParent.transform);

		resultGO.isStatic = true;
		if (groundLayer)
        {
			//resultGO.layer = MyLayer.groundLayer;
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}

	/// <summary>
	/// Recursively adds all box colliders from the descandants of root to target.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="root"></param>
	private void AddBoxColliderFromChildren(GameObject target, Transform root)
    {
		foreach (Transform child in root)
        {
			BoxCollider[] colliders = child.GetComponents<BoxCollider>();
			foreach(BoxCollider c in colliders)
            {

				// Apply collider
				BoxCollider newCollider = AddComponentFrom<BoxCollider>(c, target);
				newCollider.size = child.rotation * new Vector3(newCollider.size.x * child.localScale.x, newCollider.size.y * child.localScale.y, newCollider.size.z * child.localScale.z);

				Vector3 rotatedCenter = child.rotation * new Vector3(c.center.x, c.center.y, c.center.z);
				newCollider.center = (child.position - target.transform.position) + new Vector3(rotatedCenter.x * child.localScale.x, rotatedCenter.y * child.localScale.y, rotatedCenter.z * child.localScale.z); // here local pos is used

				newCollider.material = null;
			}

			AddBoxColliderFromChildren(target, child);
		}
    }

	private T AddComponentFrom<T>(T original, GameObject destination) where T : Component
	{
		System.Type type = original.GetType();
		var dst = destination.AddComponent(type) as T;
		var fields = type.GetFields();
		foreach (var field in fields)
		{
			if (field.IsStatic) continue;
			field.SetValue(dst, field.GetValue(original));
		}
		var props = type.GetProperties();
		foreach (var prop in props)
		{
			if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
			prop.SetValue(dst, prop.GetValue(original, null), null);
		}
		return dst as T;
	}
}
#endif