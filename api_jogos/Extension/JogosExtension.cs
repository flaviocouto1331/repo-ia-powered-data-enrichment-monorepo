using api_jogos.Data.Repositories;
using api_jogos.Services;

namespace api_jogos.Extension
{
    public static class JogosExtension
    {
        public static IServiceCollection AddJogosExtension(this IServiceCollection services) 
        {
            services.AddScoped<IJogosRepository, JogosRepository>();
            services.AddTransient<IJogosService, JogosService>();
            return services;
        }
    }
}
