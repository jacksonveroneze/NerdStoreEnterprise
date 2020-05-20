﻿using AutoMapper;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;

namespace NSE.Identidade.API.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDataResponse>();
            
            CreateMap<UsuarioRegistro, ApplicationUser>()
                .ForMember(dest => dest.UserName, o => o.MapFrom(src => src.Email));
        }
    }
}