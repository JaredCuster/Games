using Games.Services;

namespace Games
{
    public static class Ioc
    {
        public static IServiceCollection AddApplicationDepencyGroup(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBattleService, BattleService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IInfoService, InfoService>();

            services.AddScoped<IAdminDataService, AdminDataService>();
            services.AddScoped<IBattleDataService, BattleDataService>();
            services.AddScoped<ICharacterDataService, CharacterDataService>();
            services.AddScoped<IInfoDataService, InfoDataService>();

            return services;
        }
    }
}
