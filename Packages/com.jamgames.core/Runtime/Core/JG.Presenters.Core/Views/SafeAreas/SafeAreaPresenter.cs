using System.Runtime.InteropServices;
using Core.JG.Presenters.Core.Interfaces.Views.SafeAreas;
using Core.JG.Views.UI.Core.Interfaces.SafeAreas;
using UnityEngine;

namespace Core.JG.Presenters.Core.Views.SafeAreas
{
    public class SafeAreaPresenter : ISafeAreaPresenter
    {
        const bool SimIphoneX = false;
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsMobile();
#endif

        // Rect _landscapeIphoneX = new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f);  // Landscape
        Rect _landscapeIphoneX = new Rect(63f / 1125f, 132f / 2436f, 1062f / 1125f, 2172f / 2436f);  // Portrait
        
        public SafeAreaPresenter(ISafeAreaView[] safeZoneViews)
        {
            var safeArea = Screen.safeArea;
            var isWebGLMobile = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            isWebGLMobile = IsMobile();
#endif
            if (Application.isEditor && SimIphoneX || isWebGLMobile)
            {
                var nsa = _landscapeIphoneX;
                safeArea = new Rect (Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            }
            
            foreach (var safeZoneView in safeZoneViews)
            {
                safeZoneView.ApplySafeArea(safeArea);
            }
        }
    }
}