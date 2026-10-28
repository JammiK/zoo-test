using Core.JG.Views.UI.Core.Interfaces.SafeAreas;
using UnityEngine;

namespace Core.JG.Views.UI.Core.SafeAreas
{
    public class LeftOffsetSafeAreaView : MonoBehaviour, ISafeAreaView
    {
        [SerializeField] bool _min = true;
        [SerializeField] bool _max = true;
        [SerializeField] bool _invert = false;
        
        public void ApplySafeArea(Rect safeRect)
        {
            var sign = _invert ? -1 : 1; 
            var panel = GetComponent<RectTransform>();
            if (_min)
            {
                panel.offsetMin += new Vector2(sign * safeRect.position.x, 0);
            }

            if (_max)
            {
                panel.offsetMax += new Vector2(sign * safeRect.position.x, 0);
            }
        }
    }
}