using AutoMapper;
using BooksAPI.DTOs.BookDtos;
using BooksAPI.DTOs.CategoryDtos;
using BooksAPI.DTOs.UserDtos;
using DataAccessLayer.Entities;

namespace BooksAPI.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Book, BookDto>()
            .ReverseMap();
        CreateMap<Book, AddBookDto>()
            .ReverseMap();

        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Books, 
                       opt => opt.MapFrom(src => src.Books))
            .ReverseMap();
        CreateMap<AddCategoryDto, Category>()
            .ReverseMap();

        CreateMap<User, RegisterUserDto>()
            .ReverseMap();
        CreateMap<User, LoginUserDto>()
            .ReverseMap();
    }
}
