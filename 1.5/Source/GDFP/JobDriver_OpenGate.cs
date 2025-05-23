﻿using System;
using System.Collections.Generic;
using Verse.AI;

namespace GDFP;

public class JobDriver_OpenGate : JobDriver
{
    public Building_Quackaai Gate => TargetThingA as Building_Quackaai;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedOrNull(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch, false);

        Toil toilFaceAndWait = Toils_General.Wait(90, TargetIndex.None).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, true, -0.5f);

        toilFaceAndWait.tickAction = (Action) Delegate.Combine(toilFaceAndWait.tickAction, new Action(delegate
        {
            pawn.rotationTracker.FaceTarget(job.targetA);
        }));
        toilFaceAndWait.handlingFacing = true;
        yield return toilFaceAndWait;

        Toil workToOpenDoor = Toils_General.Wait(300, TargetIndex.None)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.ClosestTouch)
            .WithProgressBarToilDelay(TargetIndex.A, 300, false, -0.5f);

        workToOpenDoor.AddFinishAction(delegate
        {
            ModLog.Log($"Ensuring Map from Job {this}");
            Gate.OpenGate();

        });
        yield return workToOpenDoor;
    }
}
