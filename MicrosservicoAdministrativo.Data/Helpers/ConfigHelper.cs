using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MicrosservicoAdministrativo.Data.Helpers
{
	[ExcludeFromCodeCoverage]
	public static class ConfigHelper
	{
		public static string GetEndpoint()
		{
			var settings = GetConfig();

			return settings["ConnectionStrings:ConexaoAzureCosmosDB:Endpoint"];
		}

		public static string GetKey()
		{
			var settings = GetConfig();

			return settings["ConnectionStrings:ConexaoAzureCosmosDB:Key"];
		}

		public static string GetDatabaseId()
		{
			var settings = GetConfig();

			return settings["ConnectionStrings:ConexaoAzureCosmosDB:DatabaseId"];
		}

		private static IConfiguration GetConfig()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			return builder.Build();
		}
	}
}