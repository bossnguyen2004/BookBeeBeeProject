using AutoMapper;
using BookBee.DTO.Account;
using BookBee.DTO.Address;
using BookBee.DTO.Role;
using BookBee.DTO.User;
using BookBee.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookBee.DTO
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserAccountDTO, Model.UserAccount>().ReverseMap();
            CreateMap<RegisterUserDTO, Model.UserAccount>().ReverseMap();
            CreateMap<RoleDTO, Model.Role>().ReverseMap();
            CreateMap<AddressDTO, Model.Address>().ReverseMap();

        }
    }
}
