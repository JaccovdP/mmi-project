using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace Feedback
{
    // Mostly inspired by https://github.com/BenoitFreslon/Vibration/blob/master/Vibration/Vibration.cs with some changes
    // https://developer.android.com/reference/android/os/Vibrator
    public class HapticFeedback : Feedback
    {
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
        public static AndroidJavaObject context;

        public static AndroidJavaClass vibrationEffect;

        public HapticFeedback(GameObject parent) : base(parent)
        {
#if UNITY_ANDROID
            if (Application.isMobilePlatform)
            {
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

                if (AndroidVersion >= 26)
                {
                    vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                    Debug.Log("Mobile device has Android version of 26 or above.");
                }
                else
                {
                    Debug.LogWarning("Mobile device has Android version below 26. Vibrations are less customized");
                }

            }
#endif
        }
        

        protected override bool FeedbackImplementation()
        {
            Vibrate(50);
            return true;
        }
        

        /// <summary>
        /// Vibrates the phone
        /// </summary>
        /// <param name="milliseconds">How long it'll vibrate for</param>
        void Vibrate(long milliseconds)
        {

            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID
                if (AndroidVersion >= 26)
                {
                    AndroidJavaObject createOneShot = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, 100);
                    vibrator.Call("vibrate", createOneShot);

                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);
                }
#endif
            }
        }

        void Vibrate(long[] pattern)
        {
            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID

                if (AndroidVersion >= 26)
                {
                    AndroidJavaObject createWaveform = vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, -1);
                    vibrator.Call("vibrate", createWaveform);

                }
                else
                {
                    vibrator.Call("vibrate", pattern, -1);
                }
#endif
            }
        }

        public override void Success()
        {
            long[] pattern = { 0, 50, 100, 50, 200 };
            Vibrate(pattern);
        }

        static int androidVersion = -1;
        public static int AndroidVersion
        {
            get
            {
                if (androidVersion == -1)
                {
                    androidVersion = 0;
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        string androidVersionString = SystemInfo.operatingSystem;
                        int sdkPos = androidVersionString.IndexOf("API-");
                        androidVersion = int.Parse(androidVersionString.Substring(sdkPos + 4, 2).ToString());
                    }
                }

                return androidVersion;
            }
        }
    }
}
