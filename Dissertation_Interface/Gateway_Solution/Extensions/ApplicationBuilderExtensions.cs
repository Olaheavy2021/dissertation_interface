using System.Diagnostics.CodeAnalysis;

namespace Gateway_Solution.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureEndpoints(this IApplicationBuilder app) =>
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz"); ;
        });
    }
}