namespace FrameWork
{
    public class PanelLoader : ResLoader
    {
        private static PanelLoader mInstance;

        public static PanelLoader Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = SafeObjectPool<PanelLoader>.Instance.Allocate();
                    return mInstance;
                }                
                return mInstance;
            }
        }

        public UnityEngine.GameObject LoadPanelPrefab(string path)
        {
            return UnityEngine.Object.Instantiate(mInstance.AssetLoad<UnityEngine.GameObject>(path));
        }

        public void Unload()
        {
            mInstance.UnloadAll();
            mInstance = null;
        }
    }
}