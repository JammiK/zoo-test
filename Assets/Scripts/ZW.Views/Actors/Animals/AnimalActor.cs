using System;
using UnityEngine;
using ZW.Views.Interfaces.Actors.Animals;

namespace ZW.Views.Actors.Animals
{
    [RequireComponent(typeof(Rigidbody))]
    public class AnimalActor : MonoBehaviour, IAnimalActor
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int JumpHash = Animator.StringToHash("Jump");

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;

        private Action<IAnimalActor> _onCollided;
        private Action<Vector3> _onObstacleHit;

        public Vector3 Position => _rigidbody.position;
        public Vector3 RenderPosition => transform.position;

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

            _animator.SetFloat(SpeedHash, horizontalVelocity.magnitude);
            if (verticalVelocity > 0f)
            {
                _animator.SetTrigger(JumpHash);
            }
        }

        public void SetCollisionListener(Action<IAnimalActor> onCollided)
        {
            _onCollided = onCollided;
        }

        public void SetObstacleListener(Action<Vector3> onObstacleHit)
        {
            _onObstacleHit = onObstacleHit;
        }

        public void SetActive(bool active)
        {
            if (!active)
            {
                _onCollided = null;
                _onObstacleHit = null;
            }

            gameObject.SetActive(active);

            if (active)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null && collision.rigidbody.TryGetComponent<IAnimalActor>(out var other))
            {
                _onCollided?.Invoke(other);
                return;
            }

            _onObstacleHit?.Invoke(collision.GetContact(0).normal);
        }
    }
}
