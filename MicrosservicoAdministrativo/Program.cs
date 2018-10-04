using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MicrosservicoAdministrativo
{
	[ExcludeFromCodeCoverage]
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
                .UseApplicationInsights()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}