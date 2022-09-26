using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
   public class PostController : Controller
   {
      private readonly ILogger<PostController> _logger;
      private readonly IMapper _mapper;
      private readonly PostService _postService = new PostService();
      public PostController(ILogger<PostController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [HttpGet]
      [Route("GetPosts")]
      public async Task<Post[]> GetPosts()
      {
         var posts = await _postService.GetPosts();
         return posts;
      }
      [HttpGet]
      [Route("GetPostById")]
      public async Task<Post> GetPostById(Guid id)
      {
         var post = await _postService.GetPostById(id);
         if (post == null)
            return null;
         return post;
      }
      [HttpGet]
      [Route("GetPostByUserId")]
      public async Task<Post[]> GetPostByUserId(Guid id)
      {
         var posts = await _postService.GetPostByUserId(id);
         return posts;
      }
      [HttpPost]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost(Guid userId, Guid[] tagIds, PostViewModel view)
      {
         var post = _mapper.Map<PostViewModel, Post>(view);
         post.UserId = userId;
         await _postService.Save(post, tagIds);
         return StatusCode(200, "Успех");
      }
      [HttpPut]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(Guid id, Guid[] tagIds, PostViewModel view)
      {
         var editPost = await _postService.GetPostById(id);
         if (editPost == null)
            return StatusCode(200, "Не Успех");
         var newPost = _mapper.Map<PostViewModel, Post>(view);
         await _postService.Update(editPost, newPost, tagIds);
         return StatusCode(200, "Успех");
      }
      [HttpDelete]
      [Route("DeletePost")]
      public async Task<IActionResult> DeletePost(Guid id)
      {
         var post = await _postService.GetPostById(id);
         if (post == null)
            return StatusCode(200, "Не Успех");
         await _postService.Delete(post);
         return StatusCode(200, "Успех");
      }
   }
}
