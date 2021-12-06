namespace Framework
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
            return new UnityEngine.GameObject();
        }

        public void Unload()
        {
            mInstance = null;
        }
    }
}
