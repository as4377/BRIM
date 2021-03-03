using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using BRIM.BackendClassLibrary;
using Microsoft.Extensions.Logging;

namespace BRIM 
{
	public class InventoryController: Controller
	{
		private readonly ILogger<InventoryController> _logger;

		private Inventory inventory;
		


		public InventoryController(ILogger<InventoryController> logger)
		{
			_logger = logger;
			_logger.LogInformation("In inventory");
			//initialize the inventory
			inventory = new Inventory();
			inventory.GetItemList();
			inventory.GetRecipeList();	

		}
		public ActionResult Index(){
			return View(new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});
		}
		
		public ActionResult Items(){
			_logger.LogInformation("Content type"+ ControllerContext.HttpContext.Request.ContentType, DateTimeOffset.Now);
			if (ControllerContext.HttpContext.Request.ContentType == "application/json")
			{
				return new JsonResult(new
				{
					Items= inventory.ItemList.AsReadOnly()
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});	
		}
		public ActionResult Recipes(){
			_logger.LogInformation("Recipe call");
			if (ControllerContext.HttpContext.Request.ContentType == "application/json"){
				List<RecipeView> reclist = this.inventory.RecipeList.Select(p=>new RecipeView()
				{
					id = p.ID,
					name = p.Name,
					baseliquor = p.BaseLiquor,
					components = p.ItemList.Select(q=>new RecipeComponent()
					{
						component = q.Item1,
						amount = q.Item2
					}).ToList()
				}).ToList();
				return new JsonResult(new{
					Recipes = reclist.AsReadOnly()//,
					//RecursionLimit=100
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});
		}
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}
		public class RecipeView
		{
			public int id {get;set;}
			public string name{get;set;}

			public string baseliquor{get;set;}
			public IReadOnlyList<RecipeComponent> components {get;set;}

		}
		public class RecipeComponent
		{
			public Item component{get;set;}
			public double amount{get;set;}
		}
		public class RecipeListViewModel
		{
			public IReadOnlyList<RecipeView> recipes { get;set;}
		}


	}
}
