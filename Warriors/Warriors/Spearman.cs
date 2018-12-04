using System;
using System.Collections.Generic;
using System.Text;
using Warriors.Weapons;

namespace Warriors.Warriors
{
    public class Spearman : Warrior
    {
        public Spearman(IWeapon weapon) : base(weapon)
        {
        }

        protected override string Name  => "Spearman";


        public override void Attack()
        {

            base.Attack();

        }
    }
}
