using System;
using System.Collections.Generic;
using System.Text;

namespace Warriors.Weapons
{
    public abstract class Spear : IWeapon
    {
        public WeaponType Type => throw new NotImplementedException();

        public WeaponMaterial Material => throw new NotImplementedException();

        public virtual void Use()
        {
            
        }
    }
}
