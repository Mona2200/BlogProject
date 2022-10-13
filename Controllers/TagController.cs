using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
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
      [Route("AddTag")]
      public async Task<IActionResult> AddTag(AddTagViewModel view)
      {
         var tag = _mapper.Map<AddTagViewModel, Tag>(view);
         await _tagService.Save(tag);
         return Ok();
      }
      [Authorize(Roles = "moder")]
      [HttpPut]
      [Route("EditTag")]
      public async Task<IActionResult> EditTag(Guid id, AddTagViewModel view)
      {
         var editTag = await _tagService.GetTagById(id);
         if (editTag == null)
            return BadRequest();
         var newTag = _mapper.Map<AddTagViewModel, Tag>(view);
         await _tagService.Update(editTag, newTag);
         return Ok();
      }
      [Authorize(Roles = "moder")]
      [HttpDelete]
      [Route("DeleteTag")]
      public async Task<IActionResult> DeleteTag(Guid id)
      {
         var tag = await _tagService.GetTagById(id);
         if (tag == null)
            return BadRequest();
         await _tagService.Delete(tag);
         return Ok();
      }
   }
}
