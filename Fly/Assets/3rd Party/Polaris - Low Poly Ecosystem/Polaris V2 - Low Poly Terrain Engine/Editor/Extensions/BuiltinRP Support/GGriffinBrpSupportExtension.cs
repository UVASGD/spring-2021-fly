#if GRIFFIN && UNITY_EDITOR
using UnityEngine;

namespace Pinwheel.Griffin.BuiltinRP.GriffinExtension
{
    public static class GriffinBrpSupport
    {
        public static string GetExtensionName()
        {
            return "Built-in Render Pipeline Support";
        }

        public static string GetPublisherName()
        {
            return "Pinwheel Studio";
        }

        public static string GetDescription()
        {
            return "Adding support for BuiltinRP.\n" +
                "Requires Unity 2018.1 or above.";
        }

        public static string GetVersion()
        {
            return "1.0.2";
        }

        public static void OpenSupportLink()
        {
            GEditorCommon.OpenEmailEditor(
                GCommon.SUPPORT_EMAIL,
                "[Polaris V2] BuiltinRP Support",
                "YOUR_MESSAGE_HERE");
        }

        public static void OnGUI()
        {
#if UNITY_2019
            GUI.enabled = true;
#else
            GUI.enabled = false;
#endif
            if (GUILayout.Button("Install"))
            {
                GGriffinBrpInstaller.Install();
            }

            GUI.enabled = true;
        }
    }
}
#endif
