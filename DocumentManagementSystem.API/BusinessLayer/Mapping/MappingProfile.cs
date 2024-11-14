using AutoMapper;
using DataAccessLayer.Entity;
using Model;

namespace BusinessLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Document, DocumentModel>().ReverseMap();
        }
    }
}
