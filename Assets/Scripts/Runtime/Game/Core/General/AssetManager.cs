using ScriptableObjects;
using UnityEngine;

namespace SimplePoker.Core.General
{
    public class AssetManager : Singleton<AssetManager>
    {
        public AssetCollection AssetCollection;

        void Awake()
        {
            AssetCollection = Resources.Load<AssetCollection>("Assets");
        }
        public GameObject GetPrefab(string name)
        {
            foreach (var asset in AssetCollection.Assets)
            {
                if (asset.Name.Equals(name))
                {
                    return asset.Prefab;
                }
            }

            return null;
        }

        public Sprite GetSprite(string name)
        {
            foreach (var asset in AssetCollection.ImageAssets)
            {
                if (asset.Name.Equals(name))
                {
                    return asset.image;
                }
            }

            return null;
        }
    }
}