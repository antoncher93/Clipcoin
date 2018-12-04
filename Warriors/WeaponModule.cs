using System;
using System.Collections.Generic;
using System.Text;
using Ninject.Modules;
using Warriors.Weapons;
using Warriors.Weapons.Wood;

namespace Warriors
{
    public class WeaponModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IWeapon>().To<WoodSword>();
        }
    }
}
