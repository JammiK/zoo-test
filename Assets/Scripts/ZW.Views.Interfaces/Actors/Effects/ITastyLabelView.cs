using UnityEngine;
using ZW.Views.Interfaces.Common;

namespace ZW.Views.Interfaces.Actors.Effects
{
    public interface ITastyLabelView : IPoolableView
    {
        void SetText(string text);
        void SetAnchor(Vector3 position);
    }
}
