using UnityEditor;
using UnityEngine;

namespace Core.JG.Extensions.Editor.Buttons
{
    public class CreateScreenshotButton
    {
        [MenuItem("[JG]/Screenshots/Create Screenshot")]
        public static void CreateScreenshot()
        {
            ScreenCapture.CaptureScreenshot("Screenshot.png");
        }
    }
}