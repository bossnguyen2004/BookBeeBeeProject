using AutoMapper;
using BookBee.DTO.Account;
using BookBee.DTO.Address;
using BookBee.DTO.Author;
using BookBee.DTO.Book;
using BookBee.DTO.Employee;
using BookBee.DTO.OrderVoucher;
using BookBee.DTO.PaymentMethod;
using BookBee.DTO.Publisher;
using BookBee.DTO.Role;
using BookBee.DTO.Supplier;
using BookBee.DTO.Tag;
using BookBee.DTO.User;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookStack.DTO.Cart;
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
			CreateMap<AuthorDTO, Model.Author>().ReverseMap();
			CreateMap<PublisherDTO, Model.Publisher>().ReverseMap();
			CreateMap<SupplierDTO, Model.Supplier>().ReverseMap();
			CreateMap<TagDTO, Model.Tag>().ReverseMap();
			CreateMap<VoucherDTO, Model.Voucher>().ReverseMap();
			CreateMap<PaymentMethodDTO, Model.PaymentMethod>().ReverseMap();
            CreateMap<EmployeeDTO, Model.Employee>().ReverseMap();
            CreateMap<OrderVoucherDTO, Model.OrderVoucher>().ReverseMap();
            CreateMap<BookDTO, Model.Book>().ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl)).ReverseMap();
             //.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image));
            CreateMap<CartDTO, Model.Cart>().ReverseMap();
		}
    }
}
