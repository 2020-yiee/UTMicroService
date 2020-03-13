using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatisticAPIService.Repository;

namespace StatisticAPIService
{
    public class Program
    {
        private static readonly IStatisticRepository repository = new StatisticRepositoryImpl();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            //Thread thread = new Thread(statisticData);
            //thread.Start();
        }


        public static void statisticData()
        {
            repository.updateStatisticData();
            Thread.Sleep(2*60*1000);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
