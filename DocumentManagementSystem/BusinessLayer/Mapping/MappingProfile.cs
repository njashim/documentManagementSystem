using AutoMapper;
using Model;
using System.Reflection.Metadata;

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