using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Clipcoin.Phone.Services.Signals;
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

            SignalNotifier n = new SignalNotifier();

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
