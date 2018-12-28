using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqTest.Classes;
using LinqTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinqTest
{
    class Program
    {
        static void Main(string[] args)
        {

            IServiceCollection _serviceCollection = new ServiceCollection();
            ConfigureService(_serviceCollection);

            Worker worker = new Worker(_serviceCollection);

            Console.WriteLine(worker.DoOne());
            Console.WriteLine(worker.DoTwo());

            Console.ReadKey();
        }

        static void ConfigureService(IServiceCollection serviceCollection)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            serviceCollection
                .AddLogging();
                //.AddSingleton<ILoggerFactory>(loggerFactory);
        }

        
    }
}
