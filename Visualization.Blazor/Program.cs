using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Visualization.Blazor
{
    public class Program
    {

        private static int _port = 5000;
        private const string _portArg = @"port";


        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) {
            for(int i = 0; i < args.Length; i++)
                switch (args[i]) {
                    case _portArg:
                        if (int.TryParse(args[i + 1], out int newPort)) {
                            _port = newPort;
                            i++;
                        }
                        break;
                    default:
                        Console.WriteLine($"Argument {args[i]} is not defined");
                        break;
                }

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseUrls($"http://*:{_port}");
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
