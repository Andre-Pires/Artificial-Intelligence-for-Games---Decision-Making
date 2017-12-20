using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander(float volatility, float turnSpeed, float maxAcceleration)
        {
            this.Target = new KinematicData();
            this.Volatility = volatility;
            this.TurnSpeed = turnSpeed;
            this.MaxAcceleration = maxAcceleration;
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float Volatility { get; private set; }
        public float TurnSpeed { get; private set; }

        public override MovementOutput GetMovement()
        {
            // Make sure we have a target
            if (!(this.Target.position.sqrMagnitude > 0))
            {
                this.Target.position = new Vector3(this.Character.position.x + this.Volatility, this.Character.position.y, this.Target.position.z);
            }

            Vector3 offSet = this.Target.position - this.Character.position;

            float angle;

            if (offSet.x*offSet.x + offSet.z*offSet.z > 0)
            {
                angle = (float) Math.Atan2(offSet.z, offSet.x);

            }
            else
            {
                angle = 0;
            }

            this.Target.position = this.Character.position;

            this.Target.position = new Vector3(
                this.Target.position.x + this.Volatility * (float)Math.Cos(angle) + RandomHelper.RandomBinomial(this.TurnSpeed),
                this.Target.position.y,
                this.Target.position.z + this.Volatility * (float)Math.Sin(angle) + RandomHelper.RandomBinomial(this.TurnSpeed)
                );

            return base.GetMovement();
        }
    }
}
