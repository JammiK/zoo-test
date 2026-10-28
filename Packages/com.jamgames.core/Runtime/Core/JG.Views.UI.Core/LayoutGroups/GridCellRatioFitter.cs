using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.LayoutGroups
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridCellRatioFitter : MonoBehaviour
    {
        enum FitType
        {
            WidthControlsHeight,
            HeightControlsWidth
        }

        [SerializeField] FitType _fitType;
        [SerializeField] float _ratio = 1;
        [SerializeField] bool _freezeBaseAxis;

        void Start()
        {
            UpdateGeometry();
        }

        void UpdateGeometry()
        {
            if (_freezeBaseAxis)
            {
                UpdateGeometryWithFreeze();
            }
            else
            {
                UpdateGeometryWithoutFreeze();
            }
        }

        void UpdateGeometryWithoutFreeze()
        {
            var grid = GetComponent<GridLayoutGroup>();
            var cellSize = grid.cellSize;
            var rectTransform = GetComponent<RectTransform>();

            if (rectTransform.rect.size.x < 1 || rectTransform.rect.size.y < 1)
            {
                return;
            }
            
            var width = _fitType == FitType.WidthControlsHeight 
                ? (rectTransform.rect.size.x - grid.spacing.x * (grid.constraintCount - 1) - grid.padding.left - grid.padding.right) / grid.constraintCount
                : cellSize.x;
            
            var height =_fitType == FitType.HeightControlsWidth
                ? (rectTransform.rect.size.y - grid.spacing.y * (grid.constraintCount - 1) - grid.padding.top - grid.padding.bottom) / grid.constraintCount
                : cellSize.y;

            if (_fitType == FitType.HeightControlsWidth)
            {
                width = height * _ratio;
            } else if (_fitType == FitType.WidthControlsHeight)
            {
                height = width / _ratio;
            }
            
            grid.cellSize = new Vector2(width, height);
        }
        
        void UpdateGeometryWithFreeze()
        {
            var grid = GetComponent<GridLayoutGroup>();
            var cellSize = grid.cellSize;
            var rectTransform = GetComponent<RectTransform>();

            if (rectTransform.rect.size.x < 1 || rectTransform.rect.size.y < 1)
            {
                return;
            }
            
            var width = cellSize.x;
            
            var height = cellSize.y;

            if (_fitType == FitType.HeightControlsWidth)
            {
                width = height * _ratio;
            } else if (_fitType == FitType.WidthControlsHeight)
            {
                height = width / _ratio;
            }

            var spacing =
                (rectTransform.rect.size.x - grid.padding.left - grid.padding.right - width * grid.constraintCount) /
                (grid.constraintCount - 1);
            
            grid.spacing = new Vector2(spacing, spacing);
            grid.cellSize = new Vector2(width, height);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            UpdateGeometry();
        }

        // [Button("Update Geometry")]
        // void UpdateButton()
        // {
        //     UpdateGeometry();
        // }
#endif
        
    }

}