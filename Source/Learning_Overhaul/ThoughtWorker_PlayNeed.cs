using RimWorld;
using Verse;

namespace Learning_Overhaul
{
    public class ThoughtWorker_PlayNeed : ThoughtWorker
    {
        
        protected override ThoughtState CurrentStateInternal(Pawn Pawn)
        {
            
            int overAge = 13;
            
                if(Pawn.ageTracker.AgeBiologicalYears >= overAge) 
                {
                    return ThoughtState.Inactive;
                }

            var joyNeed = Pawn.needs.joy;
            
                if (joyNeed == null)
                {
                    
                    return ThoughtState.Inactive;
                    
                }

            float playLevel = joyNeed.CurLevelPercentage;
                
                if (playLevel < 0.1f)
                {
                    return ThoughtState.ActiveAtStage(2); //Very Bored -20
                }

                else if (playLevel < 0.2f)
                {
                    return ThoughtState.ActiveAtStage(1); //Quite Bored - 12
                }
                else if (playLevel < 0.4f)
                {
                    return ThoughtState.ActiveAtStage(0); //Tiny bit bored -5
                }
                
            return ThoughtState.Inactive;
        }
    }
}