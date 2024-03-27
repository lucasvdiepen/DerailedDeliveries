using DerailedDeliveries.Framework.Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    public class BoxGrabbable : Grabbable
    {
        private protected override void UseGrabbable(Interactor interactor)
        {
            if (!IsBeingInteracted)
            {
                //Check for shelf / usecase (new class has to be made)


                return;
            }

            base.UseGrabbable(interactor);
        }
    }
}