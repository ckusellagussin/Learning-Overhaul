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
            {
                return ThinkResult.NoJob;
            }

            if (pawn.needs.joy.CurLevelPercentage > 0.5f)
            {
                return ThinkResult.NoJob;
                
            }

            Pawn playmate = FindPlaymate(pawn);

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
                    p.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && // Can talk/interact
                    p.health.capacities.CapableOf(PawnCapacityDefOf.Moving) // Can move
            );
        }

        private bool IsValidRecreationalBuilding(Thing thing, Pawn pawn)
        {

            if (thing.def.category != ThingCategory.Building)
            {
                return false;
            }

            if (thing.def.IsDrug || thing.def.ingestible != null)
            {
                return false;
            }

            if (!thing.IsSociallyProper(pawn) || thing.IsForbidden(pawn))
            {
                return false;
            }

            if (!pawn.CanReach(thing, PathEndMode.InteractionCell, Danger.None))
            {
                return false;
            }

            return true;
        }

        private Thing FindRecreationalThing(Pawn pawn)
        {

            var allJoySource = pawn.Map.listerThings.AllThings.Where(Building => IsValidRecreationalBuilding(Building)
                && IsBuildingAvailable(Building, pawn)).OrderByDescending(Building => )


        }

        private bool IsBuildingAvailable(Thing thing, Pawn pawn)
        {
            CompPowerTrader powerComp = thing.TryGetComp <CompPowerTrader>();
            if (powerComp != null && !powerComp.PowerOn)
            {
                return false;
            }

            return pawn.CanReserve(thing);
        }
        
        

        private int GetMaxUsersforBuilding(Thing thing)
        {

            if (thing.def.hasInteractionCell)
            {
                return 1;
            }

            if (thing.def.hasInteractionCell)
            {
                return 2;
            }

            return 1;
        }
        
        private float GetBuildingPlayfulnessScore(Thing thing)
        {
            float score = 0.5f; // Base score

            // Check for JoyGiver comp to determine playfulness
            foreach (var joyGiverDef in DefDatabase<JoyGiverDef>.AllDefs)
            {
                if (joyGiverDef.Worker.CanBeGivenTo(thing))
                {
                    score += GetJoyKindScore(joyGiverDef.joyKind);
                    break; // Use the first matching joy giver
                }
            }
    }
        private float GetJoyKindScore(JoyKindDef joyKind)
        {
            if (joyKind == JoyKindDefOf.Gaming_Board) return 0.4f;
            if (joyKind == JoyKindDefOf.Gane) return 0.3f;
            if (joyKind == JoyKindDefOf.Social) return 0.5f;
            if (joyKind == JoyKindDefOf.Meditative) return -0.2f;
            if (joyKind == JoyKindDefOf.Gluttonous) return 0.1f;
            
            return 0.2f;
        }
}