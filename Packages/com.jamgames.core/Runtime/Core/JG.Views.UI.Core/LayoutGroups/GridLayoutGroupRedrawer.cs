using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.LayoutGroups
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridLayoutGroupRedrawer : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup _layoutGroup;

        void OnEnable()
        {
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;
        }

        void OnValidate()
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<GridLayoutGroup>();
            }
        }
    }
}