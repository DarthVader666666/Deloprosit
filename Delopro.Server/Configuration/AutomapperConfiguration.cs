using AutoMapper;

using Delopro.Server.Models;
using Delopro.Data.Entities;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Delopro.Server.Configurations
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
                        .ForMember(dest => dest.Password, opts => opts.MapFrom(src => EncodeUTF8(src.Password)))
                        .ForMember(dest => dest.IsConfirmed, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<Chapter, ChapterResponseModel>()
                        .ForMember(dest => dest.Themes, opts => opts.MapFrom(src => 
                            src.Themes == null 
                            ? Array.Empty<ThemeResponseModel>() 
                            : src.Themes.Select(x => new ThemeResponseModel
                            {
                                ThemeId = x.ThemeId,
                                UserId = x.UserId,
                                ChapterId = x.ChapterId,
                                ThemeTitle = x.ThemeTitle,
                                Content = x.Content,
                                DateCreated = x.DateCreated,
                                DateDeleted = x.DateDeleted
                            }
                        ).ToArray()));

                    autoMapperConfig.CreateMap<Chapter, ChapterNode>()
                        .ForMember(dest => dest.Key, opts => opts.MapFrom(src => $"{src.ChapterId}"))
                        .ForMember(dest => dest.Label, opts => opts.MapFrom(src => src.ChapterTitle))
                        .ForMember(dest => dest.Children, opts => opts.MapFrom(src =>
                            src.Themes == null
                            ? Array.Empty<ThemeNode>()
                            : src.Themes.Select(x => new ThemeNode
                            {
                                Key = $"{x.ChapterId}-{x.ThemeId}",
                                Label = x.ThemeTitle,
                                Data = $"/chapters/{x.ChapterId}/{x.ThemeId}"
                            }).ToArray()));

                    autoMapperConfig.CreateMap<ChapterUpdateModel, Chapter>().ForMember(dest => dest.Themes, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<ThemeUpdateModel, Theme>()
                        .ForMember(dest => dest.Content, opts => opts.MapFrom(src => src.Content != null ? src.Content.Replace("&nbsp;", " ") : null));
                    autoMapperConfig.CreateMap<ThemeCreateModel, Theme>()
                        .ForMember(dest => dest.Content, opts => opts.MapFrom(src => src.Content != null ? src.Content.Replace("&nbsp;", " ") : null));

                    autoMapperConfig.CreateMap<MessageForm, Message>();
                    autoMapperConfig.CreateMap<Message, MessageResponseModel>()
                        .ForMember(dest => dest.Contacts, opts => opts.MapFrom(src => GetContacts(src.Email, src.Phone)));
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

        private static string? GetContacts(string? email, string? phone)
        {
            return (email, phone) switch
            {
                (var e, var p) when !e.IsNullOrEmpty() && !p.IsNullOrEmpty() => $"Email: {e}\n\rТел.: {p}",
                (var e, var p) when e.IsNullOrEmpty() && !p.IsNullOrEmpty() => $"Тел.: {p}",
                (var e, var p) when !e.IsNullOrEmpty() && p.IsNullOrEmpty() => $"Email: {e}",
                _ => null
            };
        }
    }
}