using UnityEngine;

namespace Com.OneSignal.Android
{
    static class OneSignalIOSInit
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if(!Application.isEditor)
                OneSignal.RegisterPlatform(new OneSignalAndroid());
        }
    }
}
