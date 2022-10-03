using BlogProject.Models;
using BlogProject.ViewModels;
using AutoMapper;

namespace BlogProject
{
   public class MappingProfile : Profile
   {
      public MappingProfile()
      {
         CreateMap<RegisterViewModel, User>();
         CreateMap<AddPostViewModel, Post>();
         CreateMap<AddCommentViewModel, Comment>();
         CreateMap<AddTagViewModel, Tag>();

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"));
         CreateMap<Post, PostViewModel>();
      }
   }
}
