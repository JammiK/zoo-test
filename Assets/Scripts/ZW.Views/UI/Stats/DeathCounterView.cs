using TMPro;
using UnityEngine;
using ZW.Views.Interfaces.UI.Stats;

namespace ZW.Views.UI.Stats
{
    public class DeathCounterView : MonoBehaviour, IDeathCounterView
    {
        [SerializeField] private string _foodChainId;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _count;

        public string FoodChainId => _foodChainId;

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void SetCount(int count)
        {
            _count.text = count.ToString();
        }
    }
}
