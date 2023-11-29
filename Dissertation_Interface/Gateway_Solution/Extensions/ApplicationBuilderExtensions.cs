namespace Gateway_Solution.Extensions
{
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