using System;
using RimWorld;
using HarmonyLib;
using Verse;

namespace Learning_Overhaul
{
    public class LearningOverhaulMain : Mod
    {
        public LearningOverhaulMain(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("CKG.LearningOverhaul");
            harmony.PatchAll();
            
            Log.Message("Learning Overhaul: Harmony patches applied");
        }
    }

    [StaticConstructorOnStartup]
    public static class LearningOverhaulStartup
    {
        static LearningOverhaulStartup()
        {
            ThoughtDef thoughtDef = DefDatabase<ThoughtDef>.GetNamedSilentFail("LowPlay");
            if (thoughtDef != null)
            {
                Log.Message("Learning Overhaul: SUCCESS - LowPlay thought def found!");
            }
            else
            {
                Log.Error("Learning Overhaul: FAILED - LowPlay thought def NOT found!");
                
                // List ALL loaded thought defs to see what's available
                Log.Message("=== ALL LOADED THOUGHT DEFS ===");
                foreach (var def in DefDatabase<ThoughtDef>.AllDefs)
                {
                    Log.Message($"Thought def: {def.defName}");
                }
                Log.Message("=== END THOUGHT DEFS ===");
            }
        }
    }
}