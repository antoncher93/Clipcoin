using System;
using System.Collections.Generic;
using System.Text;

namespace Warriors.Weapons.Services
{
    public interface IWeaponService
    {
        IWeaponFactory GetFactory(WeaponMaterial type);
    }
}
