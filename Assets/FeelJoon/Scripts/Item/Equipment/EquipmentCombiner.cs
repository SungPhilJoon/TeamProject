using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class EquipmentCombiner
    {
        #region Variables
        public readonly Dictionary<int, Transform> rootBoneDictionary = new Dictionary<int, Transform>();

        private readonly Transform rootTransform;

        #endregion Variables

        public EquipmentCombiner(GameObject rootGo)
        {
            rootTransform = rootGo.transform;
            TraverseHierarchy(rootTransform);
        }

        #region Helper Methods
        public Transform AddLimb(GameObject itemGo, List<string> boneNames)
        {
            Transform limb = ProcessBoneObject(itemGo.GetComponentInChildren<SkinnedMeshRenderer>(), boneNames);
            limb.SetParent(rootTransform);

            return limb;
        }

        private Transform ProcessBoneObject(SkinnedMeshRenderer renderer, List<string> boneNames)
        {
            Transform itemTransform = new GameObject().transform;

            SkinnedMeshRenderer meshRenderer = itemTransform.gameObject.AddComponent<SkinnedMeshRenderer>();

            Transform[] boneTransforms = new Transform[boneNames.Count];
            for (int i = 0; i < boneNames.Count; i++)
            {
                boneTransforms[i] = rootBoneDictionary[boneNames[i].GetHashCode()];
            }

            meshRenderer.bones = boneTransforms;
            Debug.Log(renderer.sharedMesh.ToString());
            meshRenderer.sharedMesh = renderer.sharedMesh;
            meshRenderer.materials = renderer.sharedMaterials;

            return itemTransform;
        }

        public Transform[] AddMesh(GameObject itemGo)
        {
            Transform[] itemTransforms = ProcessMeshObject(itemGo.GetComponentsInChildren<MeshRenderer>());
            return itemTransforms;
        }

        private Transform[] ProcessMeshObject(MeshRenderer[] meshRenderers)
        {
            List<Transform> itemTransforms = new List<Transform>();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                if (renderer.transform.parent != null)
                {
                    Transform parent = rootBoneDictionary[renderer.transform.parent.name.GetHashCode()];

                    GameObject itemGo = GameObject.Instantiate(renderer.gameObject, parent);
                    // itemGo.SetActive(false);
                    itemTransforms.Add(itemGo.transform);
                }
            }

            return itemTransforms.ToArray();
        }

        private void TraverseHierarchy(Transform root)
        {
            foreach (Transform child in root)
            {
                // Debug.Log()
                rootBoneDictionary.Add(child.name.GetHashCode(), child);

                TraverseHierarchy(child);
            }
        }

        #endregion Helper Methods
    }
}