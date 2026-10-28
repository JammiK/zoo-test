using TMPro;
using UnityEngine;
using ZW.Views.Interfaces.Actors.Effects;

namespace ZW.Views.Actors.Effects
{
    public class TastyLabelView : MonoBehaviour, ITastyLabelView
    {
        private static readonly Quaternion FacingCamera = Quaternion.Euler(90f, 0f, 0f);

        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _height = 2.5f;

        public void SetText(string text)
        {
            _text.text = text;
        }

        // Lifted toward the camera along its ray, so on screen the label stays exactly over the ground point.
        public void SetAnchor(Vector3 groundPoint)
        {
            var position = groundPoint + Vector3.up * _height;
            var camera = Camera.main;
            if (camera != null)
            {
                var eye = camera.transform.position;
                var eyeHeight = eye.y - groundPoint.y;
                if (eyeHeight > _height)
                {
                    position = Vector3.LerpUnclamped(groundPoint, eye, _height / eyeHeight);
                }
            }

            transform.SetPositionAndRotation(position, FacingCamera);
        }

        public void SetAlpha(float alpha)
        {
            _text.alpha = alpha;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                _text.alpha = 1f;
            }

            gameObject.SetActive(active);
        }
    }
}
