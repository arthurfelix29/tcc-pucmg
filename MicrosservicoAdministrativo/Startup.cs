using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicrosservicoAdministrativo.Core.Services;
using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using Swashbuckle.AspNetCore.Swagger;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace MicrosservicoAdministrativo
{
	[ExcludeFromCodeCoverage]
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(typeof(IRepository<Usuario, string>), typeof(UsuarioRepository));
			services.AddSingleton(typeof(IRepository<TipoEmpresa, string>), typeof(TipoEmpresaRepository));
			services.AddSingleton(typeof(IRepository<Empresa, string>), typeof(EmpresaRepository));
			services.AddSingleton(typeof(IRepository<Perfil, string>), typeof(PerfilRepository));
			services.AddSingleton(typeof(IRepository<PerfilUsuario, string>), typeof(PerfilUsuarioRepository));

			services.AddScoped<IUsuarioService, UsuarioService>();
			services.AddScoped<ITipoEmpresaService, TipoEmpresaService>();
			services.AddScoped<IEmpresaService, EmpresaService>();
			services.AddScoped<IPerfilService, PerfilService>();
			services.AddScoped<IPerfilUsuarioService, PerfilUsuarioService>();

			services.AddCors(options => { options.AddPolicy("AllowAll", builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()); });

			services.AddMvcCore().AddVersionedApiExplorer(
				options =>
				{
					options.GroupNameFormat = "'v'VVV";
					options.SubstituteApiVersionInUrl = true;
				});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddApiVersioning(options => options.ReportApiVersions = true);

			services.AddSwaggerGen(
				options =>
				{
					var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

					foreach (var description in provider.ApiVersionDescriptions)
						options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

					options.OperationFilter<SwaggerDefaultValues>();
					options.IncludeXmlComments(XmlCommentsFilePath);
				});

			services.AddDistributedRedisCache(option =>
			{
				option.Configuration = Configuration.GetConnectionString("ConexaoAzureCacheRedis");
				option.InstanceName = "master";
			});

			services.AddApplicationInsightsTelemetry(Configuration);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCors("AllowAll");

			app.UseMvc();
			app.UseSwagger();
			app.UseSwaggerUI(
				options =>
				{
					foreach (var description in provider.ApiVersionDescriptions)
					{
						options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
					}
				});
		}

		private static string XmlCommentsFilePath
		{
			get
			{
				var basePath = ApplicationEnvironment.ApplicationBasePath;
				var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";

				return Path.Combine(basePath, fileName);
			}
		}

		private static Info CreateInfoForApiVersion(ApiVersionDescription description)
		{
			var info = new Info()
			{
				Title = $"API - Microsserviço Administrativo {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = "API que representa o módulo Administrativo da FASTRANSP. ",
				Contact = new Contact() { Name = "Arthur Félix", Email = "thursilva@hotmail.com" },
			};

			if (description.IsDeprecated)
				info.Description += "<br /><br /><b>Esta versão da API foi descontinuada.</b>";

			return info;
		}
	}
}