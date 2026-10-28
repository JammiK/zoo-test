namespace ZW.Views.Interfaces.UI.Stats
{
    public interface IDeathCounterView
    {
        string FoodChainId { get; }
        void SetTitle(string title);
        void SetCount(int count);
    }
}
