using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EncodeJobOverseer;
using AppLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using AppConfig;
using AppConfig.Models;

namespace EncodeJobOverseer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args[0] == "-c") { AppConfigManager.SetConfig(args[1]); }

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.File(AppConfigManager.Model.LogFilePath)
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSystemd()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Worker>();
				})
				.UseSerilog();
	}
}