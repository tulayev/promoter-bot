using AutoMapper;
using PromoterBot.Dtos;
using PromoterBot.Models;

namespace PromoterBot.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Participant, ParticipantDto>()
                .ForMember(d => d.Promoter, o => o.MapFrom(s => s.Promoter.Name));
        }
    }
}
