using System;
using System.Collections.Generic;
using System.Text;

namespace Warriors.Weapons
{
    public interface IWeapon
    {
        void Use();
        WeaponType Type { get; }
        WeaponMaterial Material { get; }
    }
}
