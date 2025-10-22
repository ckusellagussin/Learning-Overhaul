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

        // Joy gain: 0-100% in 2 game hours max (5000 ticks)
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
                    
                    // Joy gain every tick
                    pawn.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    if (OtherChild != null)
                    {
                        OtherChild.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    }

                    // Social interaction effects
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
                // Fallback: direct social play - make this more stable
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
                
                Toil playToil = new Toil();
                playToil.initAction = () =>
                {
                    pawn.rotationTracker.FaceTarget(OtherChild);
                    // Set a longer expiry for direct play to prevent flickering
                    job.expiryInterval = 2000;
                };
                
                playToil.tickAction = () =>
                {
                    // Joy gain every tick
                    pawn.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);
                    OtherChild.needs.joy.GainJoy(JoyGainPerTick, JoyKindDefOf.Social);

                    // Social interaction effects
                    if (pawn.IsHashIntervalTick(100) && Rand.Value < 0.15f)
                    {
                        MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Play");
                    }
                };
                
                playToil.defaultCompleteMode = ToilCompleteMode.Delay;
                playToil.defaultDuration = 2000; // Longer duration for direct play
                yield return playToil;
            }

            // Final social interaction
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