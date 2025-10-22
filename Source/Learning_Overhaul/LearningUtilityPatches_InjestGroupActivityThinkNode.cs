using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;

namespace Learning_Overhaul
{
    [HarmonyPatch(typeof(ThinkTreeDef))]
    [HarmonyPatch("ResolveReferences")]
    public static class InjectThinkNodeIntoTree
    {
        static void Postfix(ThinkTreeDef __instance)
        {
            if (__instance.defName == "Humanlike")
            {
                Log.Message($"=== INJECTING ThinkNode into Humanlike think tree ===");
                
                if (__instance.thinkRoot is ThinkNode_Priority rootNode)
                {
                    Log.Message($"  Found root ThinkNode_Priority with {rootNode.subNodes?.Count ?? 0} subnodes");
                    
                    var groupActivityNode = new ThinkNode_GroupActivity();
                    
                    if (rootNode.subNodes != null)
                    {
                        int insertPosition = 1;
                        rootNode.subNodes.Insert(insertPosition, groupActivityNode);
                        
                        Log.Message($"  Successfully injected GroupActivity ThinkNode at position {insertPosition}");
                        Log.Message($"  Now have {rootNode.subNodes.Count} subnodes in think tree");
                        
                        for (int i = 0; i < rootNode.subNodes.Count; i++)
                        {
                            var node = rootNode.subNodes[i];
                            Log.Message($"    [{i}] {node.GetType().Name}");
                        }
                    }
                }
                else
                {
                    Log.Message($"  ERROR: Root node is not ThinkNode_Priority, it's {__instance.thinkRoot?.GetType().Name}");
                }
            }
        }
    }
}