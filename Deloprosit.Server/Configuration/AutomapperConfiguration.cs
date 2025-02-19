﻿using AutoMapper;

using Deloprosit.Server.Models;
using Deloprosit.Data.Entities;
using System.Text;

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
                    autoMapperConfig.CreateMap<RegisterRequestModel, User>()
                        .ForMember(dest => dest.Nickname, opts => opts.MapFrom(src => EncodeUTF8(src.Nickname)))
                        .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => EncodeUTF8(src.FirstName)))
                        .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => EncodeUTF8(src.LastName)))
                        .ForMember(dest => dest.Password, opts => opts.MapFrom(src => EncodeUTF8(src.Password)))
                        .ForMember(dest => dest.UserTitle, opts => opts.MapFrom(src => EncodeUTF8(src.Title)))
                        .ForMember(dest => dest.Country, opts => opts.MapFrom(src => EncodeUTF8(src.Country)))
                        .ForMember(dest => dest.City, opts => opts.MapFrom(src => EncodeUTF8(src.City)))
                        .ForMember(dest => dest.Info, opts => opts.MapFrom(src => EncodeUTF8(src.Info)))
                        .ForMember(dest => dest.IsConfirmed, opts => opts.Ignore());
                });

                return config.CreateMapper();
            });
        }

        private static string? EncodeUTF8(byte[]? bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                return null;
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}