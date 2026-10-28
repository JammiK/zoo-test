using UnityEngine;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Views.Actors.World
{
    public class CameraView : MonoBehaviour, ICameraView
    {
        [SerializeField] private Camera _camera;

        public void Fit(float width, float depth)
        {
            var tan = Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var height = Mathf.Max(depth * 0.5f / tan, width * 0.5f / (tan * _camera.aspect));
            _camera.transform.SetPositionAndRotation(new Vector3(0f, height, 0f), Quaternion.Euler(90f, 0f, 0f));
        }
    }
}
