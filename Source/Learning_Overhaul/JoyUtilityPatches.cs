using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Learning_Overhaul
{
    public static class JoyUtilityPatches
    {
        // Token: 0x02000005 RID: 5
        [HarmonyPatch(typeof(JoyUtility))]
        [HarmonyPatch("JoyTickCheckEnd")]
        private static class JoyUtilityPatches_JoyTickCheckEnd
        {
            // Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
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