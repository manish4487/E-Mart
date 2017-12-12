using ShoppingCart.Models.Data;
using ShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //declare list of pageviewmodel
            List<PageVM> pagesList;

            //init the list
            using (Db db=new Db())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //return view with list
            return View(pagesList);
        }

        //GET:Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }

        //POST:Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Declare slug
                string slug;
                //Init PageDTO
                PageDTO dto = new PageDTO();
                //DTO title
                dto.Title = model.Title;

                //check for and set slug
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //title and slug are unique
                if (db.Pages.Any(x=>x.Title==model.Title) || db.Pages.Any(x=>x.Slug==model.Slug))
                {
                    ModelState.AddModelError("","That title or Slug Already Exists");
                    return View(model);
                }
                //DTOthe rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            //Set tempdata messsage
            TempData["SM"] = "You have added a new page!";


            //Redirect
            return RedirectToAction("AddPage");

                //return View();
        }

        //POST:Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult Editpage(int id)
        {
            //declare pageVM
            PageVM model;
            using (Db db = new Db())
            {
                //get the page
                PageDTO dto = db.Pages.Find(id);
                //confirm page exists
                if (dto == null)
                {
                    return Content("this page does not exists");
                }
                //InIt PageVM
                model = new PageVM(dto);
            }
                //Return View with Model
                return View();
        }

        //POST:Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditGet(PageVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //get pageid
                int id = model.Id;

                //declare slug
                string slug="home";
                //find the page
                PageDTO dto = db.Pages.Find(id);
                //DTO title
                dto.Title = model.Title;
                //check slug and set
                if(model.Slug!="home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //check if slug and title are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                                    db.Pages.Where(x => x.Id !=id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or Slug Already Exists");
                    return View(model);
                }
                //DTOthe rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;                

                //Save DTO                
                db.SaveChanges();

            }

            //set tempdata message
            TempData["SM"] = "You have added a new page!";

            //redirect
            return View();
        }

    }

    

}