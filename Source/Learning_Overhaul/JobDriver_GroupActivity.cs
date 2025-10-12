using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace Learning_Overhaul
{
    public class JobDriver_GroupActivity : JobDriver
    {

        private const int PlayDuration = 1200;
        
        private Pawn otherChild => job.targetA.Thing as Pawn;
        private Thing recreationalThing => job.targetB.Thing; 


        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(otherChild, job, 1, -1, null, errorOnFailed))
                return false;

            if (recreationalThing != null)
            {
                if (!pawn.Reserve(recreationalThing, job, 1, -1, null, errorOnFailed))
                    return false;
            }

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            if (recreationalThing != null)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell);

                Toil useBuildingToil = new Toil();

                useBuildingToil.initAction = () =>
                {
                    pawn.rotationTracker.FaceCell(recreationalThing.Position);
                    if (otherChild != null &&
                        otherChild.CanReach(recreationalThing, PathEndMode.InteractionCell, Danger.None))
                    {

                        Job otherChildJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("GroupActivity"), pawn,
                            recreationalThing);
                        otherChild.jobs.StartJob(otherChildJob, JobCondition.InterruptForced);

                    }

                };
                useBuildingToil.tickAction = () =>
                {

                    float joyGain = 0.0005f;
                    pawn.needs.joy.GainJoy(joyGain, JoyKindDefOf.Social);

                    if (otherChild != null)
                    {

                        otherChild.needs.joy.GainJoy(joyGain, JoyKindDefOf.Social);

                    }

                    if (pawn.IsHashIntervalTick(100) && Rand.Value < 0.3f)
                    {

                        MoteMaker.MakeStaticMote(pawn.DrawPos, pawn.Map, ThingDefOf.Mote_Speech);

                    }
                };
                useBuildingToil.defaultCompleteMode = ToilCompleteMode.Delay;
                useBuildingToil.defaultDuration = PlayDuration;
                yield return useBuildingToil;
            }

            else
            {

                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

                Toil playToil = new Toil();
                playToil.initAction = () => pawn.rotationTracker.FaceTarget(otherChild);
                playToil.tickAction = () =>
                {
                    if (pawn.IsHashIntervalTick(100))
                    {

                        float joyGain = 0.0006f;
                        pawn.needs.joy.GainJoy(joyGain, JoyKindDefOf.Social);
                        otherChild.needs.joy.GainJoy(joyGain, JoyKindDefOf.Social);

                        if (Rand.Value < 0.15f)
                        {

                            MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Play");

                        }


                    }
                };
                playToil.defaultCompleteMode = ToilCompleteMode.Delay;
                playToil.defaultDuration = PlayDuration;
                yield return playToil;

            }

            yield return new Toil
            {
                initAction = () =>
                {
                    if (Rand.Value < 0.7f && otherChild != null)
                    {

                        pawn.interactions.TryInteractWith(otherChild, InteractionDefOf.Chitchat);

                    }

                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

        }
        
    }
}