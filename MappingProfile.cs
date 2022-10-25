using BlogProject.Models;
using AutoMapper;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;

namespace BlogProject
{
    public class MappingProfile : Profile
   {
      public MappingProfile()
      {
         CreateMap<RegisterViewModel, User>();
         CreateMap<AddPostViewModel, Post>();
         CreateMap<PostViewModel, Post>();
         CreateMap<AddCommentViewModel, Comment>();
         CreateMap<AddTagViewModel, Tag>();

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"));
         CreateMap<Post, PostViewModel>();
         CreateMap<User, RegisterViewModel>().ForMember(m => m.PasswordConfirm, opt => opt.MapFrom(u => u.Password));
      }
   }
}
