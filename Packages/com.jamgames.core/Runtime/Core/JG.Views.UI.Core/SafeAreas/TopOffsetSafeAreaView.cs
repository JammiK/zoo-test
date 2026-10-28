using Core.JG.Views.UI.Core.Interfaces.SafeAreas;
using UnityEngine;

namespace Core.JG.Views.UI.Core.SafeAreas
{
    public class TopOffsetSafeAreaView : MonoBehaviour, ISafeAreaView
    {
        [SerializeField] bool _min;
        [SerializeField] bool _max;
        [SerializeField] bool _invert;
        
        public void ApplySafeArea(Rect safeRect)
        {
            var offset = Screen.height - safeRect.position.y - safeRect.size.y;
            if (_invert)
            {
                offset *= -1;
            }
            
            var panel = GetComponent<RectTransform>();
            if (_min)
            {
                panel.offsetMin += new Vector2(0, offset);
            }
            
            if (_max)
            {
                panel.offsetMax += new Vector2(0, offset);
            }
        }
    }
}