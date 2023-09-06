using AutoMapper;
using Oasis.DTOS;
using Oasis.Models;

namespace Oasis.Helpers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Work, WorkDto>()
                .ForMember(ww=>ww.UserId ,opt=>opt.MapFrom(src=>src.User.Id))
                .ReverseMap();
        }
    }
}
