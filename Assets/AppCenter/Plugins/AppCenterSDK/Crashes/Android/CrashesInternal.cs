// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using Microsoft.AppCenter.Unity.Crashes.Models;
using Microsoft.AppCenter.Unity.Crashes;
using Microsoft.AppCenter.Unity.Internal.Utility;
using Microsoft.AppCenter.Unity.Internal;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class CrashesInternal
    {
        private static AndroidJavaClass _crashes = new AndroidJavaClass("com.microsoft.appcenter.crashes.Crashes");
        private static AndroidJavaClass _wrapperSdkExceptionManager = new AndroidJavaClass("com.microsoft.appcenter.crashes.WrapperSdkExceptionManager");

        public static void AddNativeType(List<IntPtr> nativeTypes)
        {
            nativeTypes.Add(AndroidJNI.FindClass("com/microsoft/appcenter/crashes/Crashes"));
        }

        public static void TrackException(AndroidJavaObject exception)
        {
            _wrapperSdkExceptionManager.CallStatic("trackException", exception);
        }

        public static void TrackException(AndroidJavaObject exception, IDictionary<string, string> properties)
        {
            var propertiesMap = JavaStringMapHelper.ConvertToJava(properties);
            _wrapperSdkExceptionManager.CallStatic("trackException", exception, propertiesMap);
        }

        public static AppCenterTask SetEnabledAsync(bool isEnabled)
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("setEnabled", isEnabled);
            return new AppCenterTask(future);
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("isEnabled");
            return new AppCenterTask<bool>(future);
        }

        public static void GenerateTestCrash()
        {
            // The call to the "generateTestCrash" method from native SDK wouldn't work in this
            // case because it just throws an exception which Unity automatically catches and logs,
            // without crashing the application
            // _crashes.CallStatic("generateTestCrash");

            if (Debug.isDebugBuild)
            {
                new Thread(() =>
                {
                    AndroidJNI.FindClass("Test/crash/generated/by/SDK");
                }).Start();
            }
        }

        public static AppCenterTask<bool> HasCrashedInLastSession()
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("hasCrashedInLastSession");
            return new AppCenterTask<bool>(future);
        }

        public static ErrorReport LastSessionCrashReport()
        {
            return null;
        }

        public static void DisableMachExceptionHandler()
        {
        }

        public static void SetUserConfirmationHandler(Crashes.UserConfirmationHandler handler)
        {
        }

        public static void NotifyWithUserConfirmation(Crashes.ConfirmationResult answer)
        {
        }

        public static void StartCrashes()
        {
            AppCenterInternal.Start(AppCenter.Crashes);
        }
    }
}
#endif