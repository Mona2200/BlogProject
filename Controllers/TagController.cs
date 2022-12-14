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
         _logger.LogDebug(1, "NLog находится внутри TagController");

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

         _logger.LogInformation($"Модератор просматривает все теги.");

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
         if (!ModelState.IsValid)
         {
            foreach (var key in ModelState.Keys)
            {
               if (ModelState[key].Errors.Count > 0)
                  ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
            }
            return View("~/Views/Post/AddPost.cshtml", view);
         }

         var allTags = await GetTags();
         var post = new FormPostViewModel();
         post.Post = view;
         post.AllTags = allTags;

         _logger.LogInformation($"Пользователь добавляет теги к своей публикации.");

         return View("AddTag", post);
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddTag")]
      public async Task<IActionResult> AddTag(FormPostViewModel model)
      {
         if (ModelState["AddTag.Name"].Errors.Count() > 0)
         {
            ModelState.AddModelError("AddTag.Name", $"{ModelState["AddTag.Name"].Errors[0].ErrorMessage}");
            var all = await GetTags();
            model.AllTags = all;
            return View(model);
         }

         var view = model.AddTag;
         var tag = _mapper.Map<AddTagViewModel, Tag>(view);
         await _tagService.Save(tag);
         var allTags = await GetTags();
         model.AllTags = allTags;

         _logger.LogInformation($"Пользователь создаёт тег.");

         return View(model);
      }

      [Authorize(Roles = "moder")]
      [HttpPost]
      [Route("EditTag")]
      public async Task<IActionResult> EditTag(TagViewModel view)
      {
         var editTag = await _tagService.GetTagById(view.TagId);
         if (editTag == null)
         {
            _logger.LogInformation($"Модератору не удалось отредактировать тег, т. к. тег не был найден.");

            return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
         }

         var newTag = new Tag() { Id = view.TagId, Name = view.TagName };
         await _tagService.Update(editTag, newTag);

         _logger.LogInformation($"Модератор редактирует тег.");

         return RedirectToAction("GetAllTags");
      }

      [Authorize(Roles = "moder")]
      [HttpGet]
      [Route("DeleteTag")]
      public async Task<IActionResult> DeleteTag(Guid id)
      {
         var tag = await _tagService.GetTagById(id);
         if (tag == null)
         {
            _logger.LogInformation($"Модератору не удалось удалить тег, т. к. тег не был найден.");

            return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
         }

         await _tagService.Delete(tag);

         _logger.LogInformation($"Модератор удалил тег.");

         return RedirectToAction("GetAllTags");
      }
   }
}
