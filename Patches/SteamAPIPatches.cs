// using System;
// using Everything;
// using HarmonyLib;
// using Steamworks;
// using UnityEngine;

// public class SteamAPIPatches
// {

//     [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(KeyCode) })]
//     [HarmonyPrefix]
//     public static bool Prefix(KeyCode key)
//     {
//         try
//         {
//             var stackTrace = new System.Diagnostics.StackTrace();
//             for (int i = 0; i < stackTrace.FrameCount; i++)
//             {
//                 var method = stackTrace.GetFrame(i).GetMethod();
//                 var declaringType = method.DeclaringType;

//                 Plugin.Logger.LogInfo(declaringType.Name);

//             }
//         }
//         catch (Exception ex)
//         {
//             Plugin.Logger.LogInfo(ex);
//         }
//         return true;
//     }

//     [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(string) })]
//     [HarmonyPrefix]
//     public static bool Prefix(string name)
//     {
//         try
//         {
//             var stackTrace = new System.Diagnostics.StackTrace();
//             for (int i = 0; i < stackTrace.FrameCount; i++)
//             {
//                 var method = stackTrace.GetFrame(i).GetMethod();
//                 var declaringType = method.DeclaringType;

//                 Plugin.Logger.LogInfo(declaringType.Name);

//             }
//         }
//         catch (Exception ex)
//         {
//             Plugin.Logger.LogInfo(ex);
//         }
//         return true;
//     }
// }
