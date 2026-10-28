using System;
using TMPro;
using UnityEngine;
using ZW.Views.Interfaces.Actors.Animals;

namespace ZW.Views.Actors.Animals
{
    [RequireComponent(typeof(Rigidbody))]
    public class AnimalActor : MonoBehaviour, IAnimalActor
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Vector3 _labelOffset = new(0f, 2.5f, -1f);

        private Action<IAnimalActor> _onCollided;

        public Vector3 Position => _rigidbody.position;

        public void SetPosition(Vector3 position)
        {
            _rigidbody.position = position;
            transform.position = position;
        }

        public void SetMotion(Vector3 horizontalVelocity, float verticalVelocity)
        {
            var vertical = verticalVelocity > 0f ? verticalVelocity : _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, vertical, horizontalVelocity.z);

            if (horizontalVelocity.sqrMagnitude > 0f)
            {
                _rigidbody.MoveRotation(Quaternion.LookRotation(horizontalVelocity.normalized, Vector3.up));
            }
        }

        public void SetLabel(string text)
        {
            _label.text = text;
        }

        public void SetLabelVisible(bool visible)
        {
            _label.gameObject.SetActive(visible);
        }

        public void SetCollisionListener(Action<IAnimalActor> onCollided)
        {
            _onCollided = onCollided;
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null && collision.rigidbody.TryGetComponent<IAnimalActor>(out var other))
            {
                _onCollided?.Invoke(other);
            }
        }

        private void LateUpdate()
        {
            _label.transform.SetPositionAndRotation(transform.position + _labelOffset, Quaternion.Euler(90f, 0f, 0f));
        }
    }
}
