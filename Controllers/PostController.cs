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
      private readonly TagService _tagService = new TagService();
      private readonly CommentService _commentService = new CommentService();
      public PostController(ILogger<PostController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [HttpGet]
      [Route("GetPosts")]
      public async Task<PostViewModel[]> GetPosts()
      {
         var posts = await _postService.GetPosts();
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);
            postViewModels[i].Comments = await _commentService.GetCommentByPostId(post.Id);
            i++;
         }
         return postViewModels;
      }
      [HttpGet]
      [Route("GetPostById")]
      public async Task<PostViewModel> GetPostById(Guid id)
      {
         var post = await _postService.GetPostById(id);
         if (post == null)
            return null;
         var postViewModel = _mapper.Map<Post, PostViewModel>(post);
         postViewModel.Tags = await _tagService.GetTagByPostId(post.Id);
         postViewModel.Comments = await _commentService.GetCommentByPostId(post.Id);
         return postViewModel;
      }
      [HttpGet]
      [Route("GetPostByUserId")]
      public async Task<PostViewModel[]> GetPostByUserId(Guid id)
      {
         var posts = await _postService.GetPostByUserId(id);
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);
            postViewModels[i].Comments = await _commentService.GetCommentByPostId(post.Id);
            i++;
         }
         return postViewModels;
      }
      [HttpPost]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost(Guid userId, Guid[] tagIds, AddPostViewModel view)
      {
         var post = _mapper.Map<AddPostViewModel, Post>(view);
         post.UserId = userId;
         await _postService.Save(post, tagIds);
         return StatusCode(200, "Успех");
      }
      [HttpPut]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(Guid id, Guid[] tagIds, AddPostViewModel view)
      {
         var editPost = await _postService.GetPostById(id);
         if (editPost == null)
            return StatusCode(200, "Не Успех");
         var newPost = _mapper.Map<AddPostViewModel, Post>(view);
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
