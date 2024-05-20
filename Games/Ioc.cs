using Games.Services.ControllerServices;
using Games.Services.DataServices;
using Games.Services.MessageServices;

namespace Games
{
    public static class Ioc
    {
        public static IServiceCollection AddApplicationDepencyGroup(this IServiceCollection services)
        {
            services.AddScoped<IAdminControllerService, AdminControllerService>();
            services.AddScoped<IBattleControllerService, BattleControllerService>();
            services.AddScoped<ICharacterControllerService, CharacterControllerService>();
            services.AddScoped<IInfoControllerService, InfoControllerService>();

            services.AddScoped<IAdminDataService, AdminDataService>();
            services.AddScoped<IBattleDataService, BattleDataService>();
            services.AddScoped<ICharacterDataService, CharacterDataService>();
            services.AddScoped<IInfoDataService, InfoDataService>();

            services.AddScoped<IMessageService, MessageService>();

            return services;
        }
    }
}
