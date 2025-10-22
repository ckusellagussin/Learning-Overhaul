using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace Learning_Overhaul
{
    [HarmonyPatch(typeof(Need), nameof(Need.LabelCap), MethodType.Getter)]
    public static class ChangeRecreationLabelForChildrenPatch
    {
        private static readonly FieldInfo pawnField = typeof(Need).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        
        static void Postfix(Need __instance, ref string __result)
        {
            if (__instance.def.defName == "Joy")
            {
                Pawn pawn = pawnField?.GetValue(__instance) as Pawn;
                if (pawn != null && pawn.ageTracker.AgeBiologicalYears < 13)
                {
                    __result = "Play";
                }
            }
        }
    }
   
}
    
