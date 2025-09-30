using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Learning_Overhaul
{
    [HarmonyPatch(typeof(LearningUtility))]
    [HarmonyPatch("LearningRateFactor")]
    public static class LearningUtilityPatches_LearningRateFactor
    {

        private static void Postfix(Pawn pawn, ref float __result)
        {
            if (pawn.needs.joy != null)
            {
                if (pawn.needs.joy.CurCategory == JoyCategory.Low)
                {
                    __result *= 0.75f;
                }

                if (pawn.needs.joy.CurCategory == JoyCategory.VeryLow)
                {
                    __result *= 0.25f;
                }

                if (pawn.needs.joy.CurCategory == JoyCategory.Empty)
                {
                    __result *= 0.1f;
                }
            }
        }
    }
}