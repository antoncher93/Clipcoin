using System;
using System.Collections.Generic;
using System.Text;
using Warriors.Weapons;

namespace Warriors.Warriors
{
    public abstract class Warrior : IWarrior
    {
        protected IWeapon _weapon;
        protected IWeaponFactory _factory;


        protected Warrior()
        {
            
        }

        public Warrior(IWeapon weapon)
        {
            Weapon = weapon;
        }

        protected virtual string Name { get; set; } = "Warrior";
        public virtual void Attack()
        {
            Console.Write($"{Name}: ");
            _weapon?.Use();
        }

        public void SetWeapon(WeaponMaterial material)
        {
            _weapon = _factory.GetWeapon(material);
        }

       

        public IWeapon Weapon { get => _weapon; set => _weapon = value; }
    }
}
