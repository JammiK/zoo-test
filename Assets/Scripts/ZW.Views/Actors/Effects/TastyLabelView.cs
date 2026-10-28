using TMPro;
using UnityEngine;
using ZW.Views.Interfaces.Actors.Effects;

namespace ZW.Views.Actors.Effects
{
    public class TastyLabelView : MonoBehaviour, ITastyLabelView
    {
        private static readonly Quaternion FacingCamera = Quaternion.Euler(90f, 0f, 0f);

        [SerializeField] private TMP_Text _text;
        [SerializeField] private Vector3 _offset = new(0f, 2.5f, -1f);

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetAnchor(Vector3 position)
        {
            transform.SetPositionAndRotation(position + _offset, FacingCamera);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
