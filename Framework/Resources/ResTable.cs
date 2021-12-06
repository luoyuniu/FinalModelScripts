using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public class ResTable : Table<Res>
    {
        /// <summary>
        /// 资源存储表
        /// </summary>
        public TableIndex<string, Res> NameIndex = new TableIndex<string, Res>(res => res.AssetName.ToLower());

        /// <summary>
        /// 根据searchkey获取资源
        /// </summary>
        /// <param name="resSearchKey">资源信息</param>
        /// <returns></returns>
        public Res GetResBySearchKey(ResSearchKey resSearchKey)
        {
            var assetName = resSearchKey.AssetName;

            var reses = NameIndex
                .Get(assetName);

            if (resSearchKey.AssetType != null)
            {
                reses = reses.Where(res => res.AssetType == resSearchKey.AssetType);
            }

            if (resSearchKey.OwnerBundle != null)
            {
                reses = reses.Where(res => res.OwnerBundleName == resSearchKey.OwnerBundle);
            }

            return reses.FirstOrDefault();
        }

        protected override void OnAdd(Res item)
        {
            NameIndex.Add(item);
        }

        protected override void OnRemove(Res item)
        {
            NameIndex.Remove(item);
        }

        protected override void OnClear()
        {
            NameIndex.Clear();
        }

        public override IEnumerator<Res> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(d => d.Value)
                .GetEnumerator();
        }

        protected override void OnDispose()
        {
        }
    }
}
