using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqTest.Interfaces;

namespace LinqTest.Classes
{
    public class OneRepositiry : IRepository<One>
    {
        public void Add(One item)
        {
            
        }

        public One Get()
        {
            return new One();
        }
    }
}
