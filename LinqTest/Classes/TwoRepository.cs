using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqTest.Interfaces;

namespace LinqTest.Classes
{
    public class TwoRepository : IRepository<Two>
    {
        public void Add(Two item)
        {
            
        }

        public Two Get()
        {
            return new Two();
        }
    }
}
