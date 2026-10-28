using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.Buttons
{
    [RequireComponent(typeof(Button))]
    public class ButtonAnimationView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Button _button;
        [SerializeField] Transform _targetTransform;
        [SerializeField] float _tappedScale = 0.9f;
        [SerializeField] float _animationDuration = 0.15f;
        Tweener _tweener;
        void OnEnable()
        {
            _targetTransform.localScale = Vector3.one;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable)
            {
                return;
            }
            
            _tweener?.Kill();
            // _targetTransform.localScale = Vector3.one;
            _tweener = _targetTransform.DOScale(new Vector3(_tappedScale, _tappedScale, _tappedScale), _animationDuration)
                .SetEase(Ease.OutBack);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_button.interactable)
            {
                _targetTransform.localScale = Vector3.one;
                return;
            }
            
            _tweener?.Kill();
            // _targetTransform.localScale = new Vector3(_tappedScale, _tappedScale, _tappedScale);
            _tweener = _targetTransform.DOScale(Vector3.one, _animationDuration)
                .SetEase(Ease.OutBack);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_targetTransform == null)
            {
                _targetTransform = transform;
            }
        }
#endif
    }
}