using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Learning_Overhaul
{
    public static class JoyUtilityPatches
    {
        [HarmonyPatch(typeof(JoyUtility))]
        [HarmonyPatch("JoyTickCheckEnd")]
        private static class JoyUtilityPatches_JoyTickCheckEnd
        {
            private static void Prefix(Pawn pawn, JoyTickFullJoyAction fullJoyAction = JoyTickFullJoyAction.EndJob, float extraJoyGainFactor = 1f, Building joySource = null)
            {
                if (ModsConfig.BiotechActive && pawn.needs.learning != null)
                {
                    float num = 0.5f;
                    pawn.needs.learning.Learn(1.2E-05f * num * LearningUtility.LearningRateFactor(pawn));
                }
            }
        }
    }
}