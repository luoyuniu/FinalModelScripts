namespace Framework
{
    using UnityEngine;

    public enum PanelState
    {
        Opening = 0,
        Pause = 1,
        Hide = 2,
        Closed = 3,
    }

    /// <summary>
    /// IUIPanel.
    /// </summary>
    public partial interface IPanel
    {
        Transform Transform { get; set; }
        UILevel Level { get; set; }
        PanelState State { get; set; }
        IUIData mUIData { get; set; }

        void Init(IUIData mUIData);

        void Open();

        void Show();

        void Pause();

        void Resume();

        void Hide();

        void Close(bool destroy = true);
    }
}
