using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace StandardAPI.Infraestructure.Migrations
{
    public static class MigrationRunnerSetup
    {
        public static IServiceCollection AddMigrationRunner(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(runnerBuilder => runnerBuilder
                    .AddPostgres() // CockroachDB uses PostgreSQL protocol
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MigrationRunnerSetup).Assembly).For.Migrations())
                .AddLogging(logging => logging.AddFluentMigratorConsole());

            services.AddSingleton<IVersionTableMetaData, CustomVersionTableMetaData>();

            return services;
        }
    }
}
