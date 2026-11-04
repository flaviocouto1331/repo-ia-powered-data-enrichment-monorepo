using api_jogos.Data.Dapper;

namespace api_jogos.Extension
{
    public static class DapperExtension
    {
        public static IServiceCollection AddDapperExtension(this IServiceCollection services) 
        {
            services.AddScoped<DapperContext>();
            return services;
        }
    }
}
