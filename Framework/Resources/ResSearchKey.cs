using System;
using System.Collections.Generic;

namespace Framework
{
    public class ResSearchKey : IPoolable, IPoolType
    {
        public string AssetName;

        public string OwnerBundle;

        public Type AssetType;

        public string OriginalAssetName;

        public static ResSearchKey Allocate(string assetName,Type assetType, string ownerBundleName = null)
        {
            var resSearchRule = SafeObjectPool<ResSearchKey>.Instance.Allocate();
            resSearchRule.AssetName = assetName.ToLower();
            resSearchRule.OwnerBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            resSearchRule.OriginalAssetName = assetName;
            return resSearchRule;
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<ResSearchKey>.Instance.Recycle(this);
        }

        public string GetPath()
        {
            return string.Format("/{0}/{1}", AssetType, AssetName);
        }

        public bool Match(IRes res)
        {
            if (res.AssetName == AssetName)
            {
                var isMatch = true;

                if (AssetType != null)
                {
                    isMatch = res.AssetType == AssetType;
                }

                if (OwnerBundle != null)
                {
                    isMatch = isMatch && res.OwnerBundleName == OwnerBundle;
                }

                return isMatch;
            }


            return false;
        }

        public override string ToString()
        {
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetName, OwnerBundle,
                AssetType);
        }

        void IPoolable.OnRecycled()
        {
            AssetName = null;

            OwnerBundle = null;

            AssetType = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}
