using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace Learning_Overhaul
{
    public class JobDriver_GroupActivity : JobDriver
    {
        private Pawn OtherChild => job.targetA.Thing as Pawn;
        private Thing RecreationalThing => job.targetB.Thing;
        
        private const float JoyGainPerTick = 0.0002f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(OtherChild, job, 1, -1, null, errorOnFailed))
                return false;

            if (RecreationalThing != null)
            {
                return pawn.Reserve(RecreationalThing, job, job.def.joyMaxParticipants, 0, null, errorOnFailed);
            }

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Incompletable);
            
            if (RecreationalThing != null)
            {
                this.EndOnDespawnedOrNull(TargetIndex.B, JobCondition.Incompletable);
                
                yield return Toils_Misc.FindRandomAdjacentReachableCell(TargetIndex.B, TargetIndex.C);
                yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
                yield return Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.OnCell);
                
                Toil playToil = new Toil();
                playToil.initAction = () =>
                {
                    job.locomotionUrgency = LocomotionUrgency.Walk;
                };
                
                playToil.tickAction = () =>
                {
                    pawn.rotationTracker.FaceCell(RecreationalThing.Position);
                    
                    pawn.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    if (OtherChild != null)
                    {
                        OtherChild.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    }
                    
                    if (pawn.IsHashIntervalTick(100) && Rand.Value < 0.3f)
                    {
                        MoteMaker.MakeStaticMote(pawn.DrawPos, pawn.Map, ThingDefOf.Mote_Speech);
                    }
                };
                
                playToil.handlingFacing = true;
                playToil.socialMode = RandomSocialMode.SuperActive;
                playToil.defaultCompleteMode = ToilCompleteMode.Delay;
                playToil.defaultDuration = job.def.joyDuration;
                yield return playToil;
                yield return Toils_Reserve.Release(TargetIndex.C);
            }
            else
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
                
                Toil playToil = new Toil();
                playToil.initAction = () =>
                {
                    pawn.rotationTracker.FaceTarget(OtherChild);
                    job.expiryInterval = 2000;
                };
                
                playToil.tickAction = () =>
                {
                    pawn.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    OtherChild.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    
                    if (pawn.IsHashIntervalTick(100) && Rand.Value < 0.15f)
                    {
                        MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Play");
                    }
                };
                
                playToil.defaultCompleteMode = ToilCompleteMode.Delay;
                playToil.defaultDuration = 2000; 
                yield return playToil;
            }
            
            yield return new Toil
            {
                initAction = () =>
                {
                    if (Rand.Value < 0.7f && OtherChild != null)
                    {
                        pawn.interactions.TryInteractWith(OtherChild, InteractionDefOf.Chitchat);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}