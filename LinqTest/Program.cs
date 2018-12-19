using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqTest
{
    class Program
    {
        class Creator
        {
            ConcurrentStack<int> items = new ConcurrentStack<int>();
            System.Timers.Timer timer = new System.Timers.Timer { Enabled = false, Interval = 1000 };



            public Creator()
            {
                timer.Elapsed += (s, e) =>
                {
                    Execute(PopAll());
                };
            }

            private IEnumerable<int> PopAll()
            {
                var buffer = new int[items.Count];

                items.TryPopRange(buffer, 0, items.Count);

                return buffer;
            }

            private void Execute(IEnumerable<int> values)
            {
                foreach(var v in values)
                {
                    Console.Write(v + " ");
                }


                Console.WriteLine();
            }

            public void Start()
            {
                timer.Start();
                int i = 0;
                while(true)
                {
                    Thread.Sleep(300);
                    items.Push(i);
                    i++;
                }
            }
        }

        static void Main(string[] args)
        {
            var e = GetInts();

            foreach(var i in e)
            {
                Console.WriteLine(i);
            }

            Console.ReadKey();
        }


        static IEnumerable<int> GetInts()
        {
            int i = 0;
            while(i> 0)
            {
                yield return 0;
            }
        }
    }
}
