using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace LinqTest.Classes
{
    public class Worker
    {
        private readonly IRepository<One> _oneRep;
        private readonly IRepository<Two> _twoRep;
        private ILogger _logger;


        public IServiceProvider Services { get; set; }

        public Worker(IServiceCollection serviceCollection)
        {
            ConfigureService(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            _logger = Services
                .GetRequiredService<ILoggerFactory>()
                .AddConsole()
                .CreateLogger<Worker>();

            _logger.LogInformation("Worker created successfully.");
        }

        public string DoOne()
        {
            return Services.GetRequiredService<IRepository<One>>().Get().ToString();
        }

        public string DoTwo()
        {
            return Services.GetRequiredService<IRepository<Two>>().Get().ToString();
        }

        private void ConfigureService(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IRepository<One>, OneRepositiry>()
                .AddSingleton<IRepository<Two>, TwoRepository>();
        }
    }
}
