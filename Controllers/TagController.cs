using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BlogProject.Controllers
{
    public class TagController : Controller
   {
      private readonly ILogger<TagController> _logger;
      private readonly IMapper _mapper;
      private readonly TagService _tagService = new TagService();
      public TagController(ILogger<TagController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetTags")]
      public async Task<Tag[]> GetTags()
      {
         var tags = await _tagService.GetTags();
         return tags;
      }
      [Authorize(Roles = "moder")]
      [HttpGet]
      [Route("GetAllTags")]
      public async Task<IActionResult> GetAllTags()
      {
         var tags = await _tagService.GetTags();
         var tagViewModel = new TagViewModel();
         tagViewModel.Tags = await _tagService.GetTags();

         return View("AllTags", tagViewModel);
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetTagById")]
      public async Task<Tag> GetTagById(Guid id)
      {
         var tag = await _tagService.GetTagById(id);
         return tag;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetTagByPostId")]
      public async Task<Tag[]> GetTagByPostId(Guid id)
      {
         var tags = await _tagService.GetTagByPostId(id);
         return tags;
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("Form")]
      public async Task<IActionResult> Form(AddPostViewModel view)
      {
            var allTags = await GetTags();
            var post = new FormPostViewModel();
            post.Post = view;
            post.AllTags = allTags;
            return View("AddTag", post);
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddTag")]
      public async Task<IActionResult> AddTag(FormPostViewModel model)
      {
         var view = model.AddTag;
         var tag = _mapper.Map<AddTagViewModel, Tag>(view);
         await _tagService.Save(tag);
         var allTags = await GetTags();
         model.AllTags = allTags;
         return View(model);
      }
      [Authorize(Roles = "moder")]
      [HttpPost]
      [Route("EditTag")]
      public async Task<IActionResult> EditTag(TagViewModel view)
      {
         var editTag = await _tagService.GetTagById(view.TagId);
         if (editTag == null)
            return BadRequest();
         var newTag = new Tag() { Id = view.TagId, Name = view.TagName };
         await _tagService.Update(editTag, newTag);
         return RedirectToAction("GetAllTags");
      }
      [Authorize(Roles = "moder")]
      [HttpGet]
      [Route("DeleteTag")]
      public async Task<IActionResult> DeleteTag(Guid id)
      {
         var tag = await _tagService.GetTagById(id);
         if (tag == null)
            return BadRequest();
         await _tagService.Delete(tag);
         return RedirectToAction("GetAllTags");
      }
   }
}
