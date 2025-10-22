using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Learning_Overhaul
{
    public class ThinkNode_GroupActivity : ThinkNode
    {
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if (pawn.ageTracker.AgeBiologicalYears >= 13 || pawn.needs.joy.CurLevelPercentage > 0.5f)
                return ThinkResult.NoJob;
            
            var playmate = FindPlaymate(pawn);
            if (playmate == null)
                return ThinkResult.NoJob;
            
            if (pawn.needs.joy.CurLevelPercentage >= playmate.needs.joy.CurLevelPercentage)
                return ThinkResult.NoJob;

            Log.Message($"=== GroupActivity ThinkNode for {pawn.Name} (joy: {pawn.needs.joy.CurLevelPercentage:P0}) ===");
            Log.Message($"  Found playmate: {playmate.Name} (joy: {playmate.needs.joy.CurLevelPercentage:P0})");
            
            var building = FindRecreationalBuilding(pawn);
            if (building == null)
            {
                Log.Message($"  No recreational building found - direct social play");
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("GroupActivity"), playmate);
                job.expiryInterval = 2000;
                return new ThinkResult(job, this);
            }

            Log.Message($"  Found building: {building.def.defName}");
            
            Job jobWithBuilding = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("GroupActivity"), playmate, building);
            return new ThinkResult(jobWithBuilding, this);
        }

        private Pawn FindPlaymate(Pawn pawn)
        {
            return pawn.Map.mapPawns.AllPawns.FirstOrDefault(p =>
                p != pawn &&
                p.RaceProps.Humanlike &&
                p.ageTracker.AgeBiologicalYears < 13 &&
                p.needs.joy.CurLevelPercentage < 0.5f &&
                !p.Downed &&
                !p.InMentalState &&
                !IsPawnInGroupActivity(p) && 
                pawn.CanReach(p, PathEndMode.Touch, Danger.None));
        }

        private bool IsPawnInGroupActivity(Pawn pawn)
        {
            return pawn.CurJob?.def == DefDatabase<JobDef>.GetNamed("GroupActivity");
        }

        private Thing FindRecreationalBuilding(Pawn pawn)
        {
            return pawn.Map.listerBuildings.allBuildingsColonist.FirstOrDefault(building =>
                (building.def.statBases?.Any(stat => stat.stat.defName.Contains("Joy")) == true ||
                 building.def.statBases?.Any(stat => stat.stat.defName.Contains("Recreation")) == true) &&
                pawn.CanReach(building, PathEndMode.InteractionCell, Danger.None));
        }
    }
}