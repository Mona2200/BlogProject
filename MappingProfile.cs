using BlogProject.Models;
using BlogProject.ViewModels;
using AutoMapper;

namespace BlogProject
{
   public class MappingProfile : Profile
   {
      public MappingProfile()
      {
         CreateMap<UserViewModel, User>();
         CreateMap<PostViewModel, Post>();
         CreateMap<CommentViewModel, Comment>();
         CreateMap<TagViewModel, Tag>();
      }
   }
}
