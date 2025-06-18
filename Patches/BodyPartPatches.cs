// using HarmonyLib;

// namespace Everything.BodyPartPatches;

// [HarmonyPatch(typeof(CharacterMovement), nameof(CharacterMovement.maxGravity))]
// public class Gravity
// {
//     public static bool Prefix()
//     {
//         Plugin.Logger.LogError("RAN GRAVITY");
//         if (ConfigValues.fly.value)
//         {
//             return false;
//         }
//         return true;
//     }
// }