using UnityEngine;

namespace Core.JG.Views.UI.Core.Helpers
{
    public static class MobileDevicesKeyboardHelper
    {
        // some Android-specific constants (some have changed over time)
        const string _playerClass = "com.unity3d.player.UnityPlayer";
        const string _playerActivity = "currentActivity";
        const string _playerObject = "mUnityPlayer";
        const string _keyboardDialog = "mSoftInputDialog";
        const string _keyboardRect = "android.graphics.Rect";

        public static float GetKeyboardHeight(bool includeInputField)
        {
#if UNITY_ANDROID
            return GetAndroidKeyboardHeight(includeInputField);
#else
            return GetAndroidKeyboardHeight(includeInputField);
#endif
        }

        public static float GetKeyboardRatio(bool includeInputField)
        {
#if UNITY_ANDROID
            return GetAndroidKeyboardRatio(includeInputField);
#else
                return GetIosKeyboardRatio(includeInputField);
#endif
        }

        //TODO: Test this code, please
        static float GetIosKeyboardHeight(bool includeInputField)
        {
            return TouchScreenKeyboard.area.height;
        }

        //TODO: Test this code, please
        static float GetIosKeyboardRatio(bool includeInputField)
        {
            return TouchScreenKeyboard.area.height / Screen.height;
        }

        static int GetAndroidKeyboardHeight(bool includeInputField)
        {
            using (AndroidJavaObject unityActivity =
                new AndroidJavaClass(_playerClass).GetStatic<AndroidJavaObject>(_playerActivity))
            {
                AndroidJavaObject unityPlayer = unityActivity.Get<AndroidJavaObject>(_playerObject);

                AndroidJavaObject view = unityPlayer.Call<AndroidJavaObject>("getView");
                if (view == null)
                {
                    return 0;
                }

                AndroidJavaObject dialog = unityPlayer.Get<AndroidJavaObject>(_keyboardDialog);
                while (dialog == null)
                {
                    dialog = unityPlayer.Get<AndroidJavaObject>(_keyboardDialog);
                }

                int decorHeight = 0;
                if (includeInputField)
                {
                    AndroidJavaObject decorView = dialog.Call<AndroidJavaObject>("getWindow")
                        .Call<AndroidJavaObject>("getDecorView");
                    while (decorView == null)
                    {
                        decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
                    }

                    decorHeight = decorView.Call<int>("getHeight");
                    while (decorHeight == 0)
                    {
                        decorHeight = decorView.Call<int>("getHeight");
                    }

                    // Debug.Log("decorHeight ==> [ " + decorHeight + " ] ");
                }

                using (AndroidJavaObject rect = new AndroidJavaObject(_keyboardRect))
                {
                    view.Call("getWindowVisibleDisplayFrame", rect);
                    int frameHeight = rect.Call<int>("height");
                    // Debug.Log("frameHeight ==> [ " + frameHeight + " ] ");

                    int returnVal = Screen.height - frameHeight + decorHeight;
                    // Debug.Log("returnVal ==> [ " + returnVal + " ] ");
                    return returnVal;
                }
            }
        }

        static float GetAndroidKeyboardRatio(bool includeInputField)
        {
            return (float) GetAndroidKeyboardHeight(true) / Screen.height;
        }
    }
}