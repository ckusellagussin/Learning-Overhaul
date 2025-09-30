using System;
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
        }
        
    }
}