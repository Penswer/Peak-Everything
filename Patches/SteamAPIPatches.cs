using Everything;
using HarmonyLib;
using Steamworks;

public class SteamAPIPatches
{

    // [HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.RestartAppIfNecessary))]
    // [HarmonyPrefix]
    // public static bool Prefix(AppId_t unOwnAppID)
    // {
    //     Plugin.Logger.LogInfo($"STEAM RESTART APP DISABLED. Tried to use: {unOwnAppID}");
    //     return false;
    // }
}