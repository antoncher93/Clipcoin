using System;
using System.Collections.Generic;
using System.Text;

namespace Warriors.Weapons
{
    public interface IWeaponFactory
    {
        IWeapon GetWeapon(WeaponMaterial material);
    }
}
