using System;
using System.Collections.Generic;
using System.Text;
using Warriors.Weapons.Iron;

namespace Warriors.Weapons
{
    public class SwordFactory : IWeaponFactory
    {
        public IWeapon GetWeapon(WeaponMaterial material)
        {
            switch(material)
            {
                case WeaponMaterial.Iron:
                    return new IronSword();

                case WeaponMaterial.Wood:
                    return new Wood.WoodSword();

                default:
                    return null;
            }
        }
    }
}
