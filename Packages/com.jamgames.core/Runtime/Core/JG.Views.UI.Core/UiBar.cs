using System;
using System.Globalization;
using Core.JG.Views.UI.Core.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core
{
    public class UiBar : MonoBehaviour, IUiBar
    {
        enum ValueType
        {
            None,
            Value,
            ValueMax,
            Percent
        }

        enum DigitType
        {
            Int,
            Float
        }

        [SerializeField] Image _bar;
        [SerializeField] Image _background;

        [SerializeField] TextMeshProUGUI _valueText;

        [SerializeField] string _suffix;

        [SerializeField] string _prefix;

        [SerializeField] ValueType _displayType = ValueType.Value;
        [SerializeField] DigitType _digitType = DigitType.Int;
        [SerializeField] Sprite _completedBarImage;
        [SerializeField] Sprite _inProgressBarImage;

        RectTransform _barTransform;

        float _maxValue = float.MinValue;
        float _value = float.MinValue;

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (!(Mathf.Abs(value - _maxValue) > float.Epsilon))
                {
                    return;
                }
                
                _maxValue = value;
                Redraw();
            }
        }

        public float Value
        {
            get => _value;
            set
            {
                if (!(Mathf.Abs(value - _value) > float.Epsilon))
                {
                    return;
                }
                
                _value = value;
                Redraw();
            }
        }

        void Redraw()
        {
            var fillValue = Mathf.Clamp((float) Value / (float) MaxValue, 0f, 1f);
            if (_bar.type == Image.Type.Filled)
            {
                _bar.fillAmount = fillValue;
                if (_background != null)
                {
                    _background.fillAmount = 1 - fillValue;
                }
            } 
            else if (_bar.type == Image.Type.Sliced)
            {
                if (_barTransform == null)
                {
                    _barTransform = _bar.GetComponent<RectTransform>();
                }
                
                _barTransform.anchorMin = new Vector2(0, _barTransform.anchorMin.y);
                _barTransform.anchorMax = new Vector2(fillValue, _barTransform.anchorMax.y);
                _barTransform.offsetMin = Vector2.zero;
                _barTransform.offsetMax = Vector2.zero;
            }

            if (_valueText == null)
            {
                return;
            }

            var textValue = string.Empty;
            var textMaxValue = string.Empty;

            if (_digitType == DigitType.Int)
            {
                textValue = Mathf.RoundToInt(Value).ToString();
                textMaxValue = Mathf.RoundToInt(MaxValue).ToString();
            } else if (_digitType == DigitType.Float)
            {
                textValue = Value.ToString(CultureInfo.InvariantCulture);
                textMaxValue = MaxValue.ToString(CultureInfo.InvariantCulture);
            }

            if (_valueText != null)
            {
                switch (_displayType)
                {
                    case ValueType.Value:
                        _valueText.text = $"{_prefix}{textValue} {_suffix}";
                        break;
                    case ValueType.ValueMax:
                        _valueText.text = $"{_prefix}{textValue}/{textMaxValue} {_suffix}";
                        break;
                    case ValueType.Percent:
                        var percent = Mathf.RoundToInt((float) Value / MaxValue * 100f);
                        _valueText.text = $"{_prefix} {percent} {_suffix}";
                        break;
                    default:
                        _valueText.text = string.Empty;
                        break;
                }
            }

            if (Value >= MaxValue && _completedBarImage != null)
            {
                _bar.sprite = _completedBarImage;
            }
            else if (_completedBarImage != null)
            {
                _bar.sprite = _inProgressBarImage;
            }
        }
    }
}