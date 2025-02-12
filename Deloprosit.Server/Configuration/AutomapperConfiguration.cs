using AutoMapper;

using Deloprosit.Server.Models;
using Deloprosit.Data.Entities;

namespace Deloprosit.Server.Configurations
{
    public static class AutomapperConfiguration
    {
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var config = new MapperConfiguration(autoMapperConfig =>
                {
                    autoMapperConfig.CreateMap<UserRegisterModel, User>();
                });

                return config.CreateMapper();
            });
        }
    }
}