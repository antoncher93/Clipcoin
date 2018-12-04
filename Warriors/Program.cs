using System;
using Ninject;
using Warriors.Warriors;
using Warriors.Weapons.Iron;

namespace Warriors
{
    class Program
    {
        static IKernel AppKernel;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            Warrior swordman = new Swordman();

            swordman.SetWeapon(Weapons.WeaponMaterial.Wood);

            swordman.Attack();

            Console.ReadKey();
        }
    }
}
