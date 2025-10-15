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
            if (pawn.ageTracker.AgeBiologicalYears >= 13)
                return ThinkResult.NoJob;

            if (pawn.needs.joy.CurLevelPercentage > 0.5f)
                return ThinkResult.NoJob;

            Pawn playmate = FindPlaymate(pawn);
            if (playmate != null)
            {
                Thing recreationalThing = FindRecreationalThing(pawn);
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Group Activity"), playmate, recreationalThing);
                return new ThinkResult(job, this);
            }

            return ThinkResult.NoJob;
        }

        private Pawn FindPlaymate(Pawn pawn)
        {
            return pawn.Map.mapPawns.AllPawns.Find(p =>
                p != pawn &&
                p.RaceProps.Humanlike &&
                p.ageTracker.AgeBiologicalYears < 13 &&
                p.needs.joy.CurLevelPercentage < 0.5f &&
                !p.Downed &&
                !p.InMentalState &&
                !p.Drafted &&
                pawn.CanReach(p, PathEndMode.Touch, Danger.None) &&
                p.Awake() &&
                !p.IsBurning() &&
                p.health.capacities.CapableOf(PawnCapacityDefOf.Talking) &&
                p.health.capacities.CapableOf(PawnCapacityDefOf.Moving)
            );
        }

        private Thing FindRecreationalThing(Pawn pawn)
        {
            return pawn.Map.listerBuildings.allBuildingsColonist
                .Where(building =>
                    IsLikelyRecreationalBuilding(building) &&
                    pawn.CanReserve(building) &&
                    !building.IsForbidden(pawn) &&
                    building.IsSociallyProper(pawn)
                )
                .FirstOrDefault();
        }

        private bool IsLikelyRecreationalBuilding(Building building) // Fixed casing
        {
            return building.def.statBases != null && (
                building.def.statBases.Any(stat => stat.stat.defName.Contains("Joy")) ||
                building.def.statBases.Any(stat => stat.stat.defName.Contains("Recreation")) ||
                building.def.description?.ToLower().Contains("joy") == true ||
                building.def.description?.ToLower().Contains("recreation") == true ||
                building.def.description?.ToLower().Contains("game") == true ||
                building.def.description?.ToLower().Contains("play") == true ||
                building.def.label?.ToLower().Contains("game") == true ||
                building.def.label?.ToLower().Contains("play") == true
            );
        }
    }
}