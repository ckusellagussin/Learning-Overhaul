using RimWorld;
using Verse;
using Verse.AI;

namespace Learning_Overhaul
{
    public class InteractionWorker_GroupActivity : InteractionWorker
    {

        public override float RandomSelectionWeight(Pawn Initial, Pawn Reciever)
        {

            if (!IsChild(Initial) || !IsChild(Reciever))
            {
                return 0f;
            }

            if (!hasLowPlayNeed(Initial) || !hasLowPlayNeed(Reciever))
            {
                return 0;
            }

            if (!Initial.CanReach(Reciever, PathEndMode.Touch, Danger.None))
            {
                return 0f;
            }

            float weight = 1f;
            weight *= GetPlayNeedFactor(Initial);
            weight *= GetPlayNeedFactor(Reciever);

            return weight;
        }

        private bool IsChild(Pawn pawn)
        {

            return pawn?.ageTracker?.AgeBiologicalYears < 13;

        }

        private bool hasLowPlayNeed(Pawn pawn)
        {

            var joyNeed = pawn?.needs?.joy;
            return joyNeed != null && joyNeed.CurLevelPercentage < 0.4;

        }

        private float GetPlayNeedFactor(Pawn pawn)
        {

            var joyNeed = pawn?.needs?.joy;

            if (joyNeed == null) return 0f;
            {
                return 1f - joyNeed.CurLevelPercentage;
            }
            

        }
        
    }
}