using System;
using System.Collections.Generic;
using System.Text;
using Warriors.Weapons;

namespace Warriors.Warriors
{
    public class Swordman : Warrior
    {

        public Swordman() : base(WeaponType.Sword)
        {          
            
        }

        public override void Attack()
        {
            base.Attack();
        }

        protected override string Name => "Swordman";
    }
}
