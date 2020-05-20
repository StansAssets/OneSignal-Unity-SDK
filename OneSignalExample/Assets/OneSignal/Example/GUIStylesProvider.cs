using UnityEngine;

namespace OneSignalPush
{
    static class GUIStylesProvider
    {
        static GUIStyle s_BoldLabel;

        public static GUIStyle BoldLabel
        {
            get
            {
                if (s_BoldLabel == null)
                {
                    s_BoldLabel = new GUIStyle("label");
                    s_BoldLabel.alignment = TextAnchor.MiddleCenter;
                    s_BoldLabel.fontSize = 22;
                    s_BoldLabel.fontStyle = FontStyle.Bold;
                }

                return s_BoldLabel;
            }
        }

        static GUIStyle s_ButtonLabel;

        static GUIStyle s_BottomLeftLabel;

        public static GUIStyle BottomLeftLabel
        {
            get
            {
                if (s_BottomLeftLabel == null)
                {
                    s_BottomLeftLabel = new GUIStyle("label");
                    s_BottomLeftLabel.fontSize = 14;
                    s_BottomLeftLabel.alignment = TextAnchor.LowerLeft;
                }

                return s_BottomLeftLabel;
            }
        }

        public static GUIStyle ButtonLabel
        {
            get
            {
                if (s_ButtonLabel == null)
                {
                    s_ButtonLabel = new GUIStyle("button");
                    s_ButtonLabel.fontSize = 14;
                }
                return s_ButtonLabel;
            }
        }

        static GUIStyle s_WrappedLabel;

        public static GUIStyle WrappedLabel
        {
            get
            {
                if (s_WrappedLabel == null)
                {
                    s_WrappedLabel = new GUIStyle("label");
                    s_WrappedLabel.wordWrap = true;
                }
                return s_WrappedLabel;
            }
        }
    }
}
