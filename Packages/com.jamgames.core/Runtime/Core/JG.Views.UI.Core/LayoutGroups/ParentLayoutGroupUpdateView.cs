using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.LayoutGroups
{
    public class ParentLayoutGroupUpdateView : MonoBehaviour
    {
        [SerializeField] RectTransform[] _layoutGroups;
        void Awake()
        {
            if (_layoutGroups == null || _layoutGroups.Length == 0)
            {
                _layoutGroups = GetComponentsInParent<LayoutGroup>().Select(g => g.GetComponent<RectTransform>()).ToArray();
            }
        }

        void OnEnable()
        {
            if (_layoutGroups != null)
            {
                StartCoroutine(Rebuild());
            }
        }

        void OnDisable()
        {
            if (_layoutGroups != null)
            {
                // StartCoroutine(Rebuild());
            }
        }

        IEnumerator Rebuild()
        {
            yield return new WaitForEndOfFrame();
            // foreach (var rectTransform in _layoutGroups)
            // {
            //     LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            // }
        }
    }
}
