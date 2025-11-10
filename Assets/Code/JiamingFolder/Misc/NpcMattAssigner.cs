using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NpcMattAssigner : MonoBehaviour
{
    [SerializeField] private List<Material> _clothesMatts;
    [SerializeField] private List<Material> _skinMatts;
    [SerializeField] private GameObject _NpcHolderGO;


    private void Start()
    {
        foreach(Transform child in _NpcHolderGO.transform) //ensures each npc gets the same skin color
        {
            SkinnedMeshRenderer[] renderers = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            int ranSkinRange = Random.Range(0, _skinMatts.Count);
            int ranShirtRange = Random.Range(0, _clothesMatts.Count);

            foreach (var skm in renderers)
            {
                string name = skm.name;
                if (name.Contains("Body"))
                {
                    skm.material = _clothesMatts[ranShirtRange];

                }

                else if (name.Contains("Eyes") || name.Contains("Hands") || name.Contains("Head"))
                {
                    skm.material = _skinMatts[ranSkinRange];
                }

            }
        }


    }

}
