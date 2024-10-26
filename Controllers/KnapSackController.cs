using InventoryTrackingApp.Data;
using InventoryTrackingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization.Formatters.Binary;

namespace InventoryTrackingApp.Controllers
{
    public class KnapSackController : Controller
    {
		private readonly ApplicationDbContext _context;
		public KnapSackController(ApplicationDbContext context)
		{
			_context = context;
		}
        //Helper functions to make deep copies for recursion:
        public static List<Resource> DeepCopyRes(List<Resource> originalResources)
        {
            return originalResources.Select(r => new Resource
            {
                Id = r.Id,
                Name = r.Name,
                Count = r.Count
            }).ToList();
        }

        /**
        public static List<Product> DeepCopyProd(List<Product> originalProduct)
        {
            return originalResources.Select(r => new Resource
            {
                Name = r.Name,
                Count = r.Count
            }).ToList();
        }
        **/

        //Fixes Needed:
        //1: Check that it can actually add in the base case based on resources and add until it can't add anymore (fixed)
        //2: modifying Resources_New is incidentally modifying Resources 
        public static (float, List<Product>) UksRecOld(int Deadline, List<Resource> Resources, List<Product> Products, int idx)
		{
			// Base Case: If no more products are left to consider
			if (idx == 0)
			{
                //Check resource
                float maxProfit = 0f;
                List<Product> selectedProducts = new();

                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();
                List<Resource> Resources_new = DeepCopyRes(Resources);

                while (Resources_new.FindIndex(x => x.Count < 0) == -1 && Deadline >= 0)
				{
                    Deadline -= Products[0].Time_Cost;
                    for (int j = 0; j < resources_in_product.Length; j++)
                    {
                        int matching_resource_index = Resources_new.FindIndex(x => x.Name == resources_in_product[j]);


                        if (matching_resource_index != -1)
                        {
                            Resources_new[matching_resource_index].Count =- Int32.Parse(cost_of_each[j]);

                            //If the cost is greater than the count of that resource, no more of that item will be placed in the bag.
                            //Force return notTakeProfit
                            if (Resources_new[matching_resource_index].Count < 0 || Deadline < 0)
                            {
                                return (maxProfit, selectedProducts);
                            }
                            else
                            {
                                selectedProducts.Add((Products[0]));
                                maxProfit += Products[0].Profit;
                            }

                        }
                    }
                }
				return (maxProfit, selectedProducts);
			}

			// Recursive Case: Do not take the current product
			var (notTakeProfit, notTakeProducts) = UksRecOld(Deadline, DeepCopyRes(Resources), Products, idx - 1);

			// Recursive Case: Consider taking the current product
			float takeProfit = int.MinValue;
			List<Product> takeProducts = new();
			if (Products[idx].Time_Cost <= Deadline)
			{
                // Subtract resources and check if they're sufficient
                List<Resource> Resources_new = DeepCopyRes(Resources);

                //Resources_new.AddRange(Resources);
                bool available = true;

                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();

				for (int j = 0; j < resources_in_product.Length; j++)
                {
                    int matching_resource_index = Resources.FindIndex(x => x.Name == resources_in_product[j]);


                    if(matching_resource_index != -1)
                    {
                        Resources_new[matching_resource_index].Count =
                        Resources[matching_resource_index].Count - Int32.Parse(cost_of_each[j]);

                        //If the cost is greater than the count of that resource, no more of that item will be placed in the bag.
                        //Force return notTakeProfit
                        if (Resources_new[matching_resource_index].Count < 0)
                        {
                            available = false;
                            break;
                        }

                    }
                }

				if (available)
				{
                    var (nextTakeProfit, nextTakeProducts) = UksRecOld(Deadline - Products[idx].Time_Cost, Resources_new, Products, idx);
					takeProfit = Products[idx].Profit + nextTakeProfit;
					takeProducts.AddRange(nextTakeProducts);
					takeProducts.Add(Products[idx]); // Add the current product to the selected products
				}
			}

			// Compare take vs not-take scenarios and return the better one
			if (takeProfit > notTakeProfit)
			{
				return (takeProfit, takeProducts);
			}
			else
			{
				return (notTakeProfit, notTakeProducts);
			}
		}

        public static (float, List<Product>) UksRecNewest(int Deadline, List<Resource> Resources, List<Product> Products, int idx)
        {
            // Base Case
            if (idx == 0)
            {
                float maxProfit = 0f;
                List<Product> selectedProducts = new();
                List<Resource> Resources_new = DeepCopyRes(Resources);

                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();

                while (Resources_new.FindIndex(x => x.Count < 0) == -1 && Deadline >= Products[0].Time_Cost)
                {
                    Deadline -= Products[0].Time_Cost;
                    for (int j = 0; j < resources_in_product.Length; j++)
                    {
                        int matching_resource_index = Resources_new.FindIndex(x => x.Name == resources_in_product[j]);
                        if (matching_resource_index != -1)
                        {
                            Resources_new[matching_resource_index].Count -= Int32.Parse(cost_of_each[j]);
                            if (Resources_new[matching_resource_index].Count < 0)
                                return (maxProfit, selectedProducts); // Return if resources are insufficient
                        }
                    }
                    selectedProducts.Add((new Product
                    {
                        Id = Products[0].Id,
                        Name = Products[0].Name,
                        Profit = Products[0].Profit,
                        Time_Cost = Products[0].Time_Cost,
                        ResourceNames = Products[0].ResourceNames,
                        ResourceCounts = Products[0].ResourceCounts

                    }));
                    maxProfit += Products[0].Profit;
                    Resources_new = DeepCopyRes(Resources);  // Reset for next iteration
                }
                return (maxProfit, selectedProducts);
            }

            // Recursive case: Option 1 - Do not take the product at idx
            var (notTakeProfit, notTakeProducts) = UksRecNewest(Deadline, DeepCopyRes(Resources), Products, idx - 1);

            // Recursive case: Option 2 - Take the product at idx if resources allow
            float takeProfit = float.MinValue;
            List<Product> takeProducts = new();
            if (Products[idx].Time_Cost <= Deadline)
            {
                List<Resource> Resources_Updated = DeepCopyRes(Resources);
                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();
                bool canTake = true;

                // Check and update resources if available
                for (int j = 0; j < resources_in_product.Length && canTake; j++)
                {
                    int matching_resource_index = Resources_Updated.FindIndex(x => x.Name == resources_in_product[j]);
                    if (matching_resource_index != -1)
                    {
                        Resources_Updated[matching_resource_index].Count -= Int32.Parse(cost_of_each[j]);
                        if (Resources_Updated[matching_resource_index].Count < 0)
                            canTake = false;
                    }
                }

                if (canTake)
                {
                    var (nextTakeProfit, nextTakeProducts) = UksRecNewest(Deadline - Products[idx].Time_Cost, Resources_Updated, Products, idx - 1);
                    takeProfit = Products[idx].Profit + nextTakeProfit;
                    takeProducts = nextTakeProducts;
                    takeProducts.Add((new Product
                    {
                        Id = Products[idx].Id,
                        Name = Products[idx].Name,
                        Profit = Products[idx].Profit,
                        Time_Cost = Products[idx].Time_Cost,
                        ResourceNames = Products[idx].ResourceNames,
                        ResourceCounts = Products[idx].ResourceCounts

                    }));  // Add the current product
                }
            }

            // Choose the better option
            if (takeProfit > notTakeProfit)
                return (takeProfit, takeProducts);
            else
                return (notTakeProfit, notTakeProducts);
        }
        public static (float, List<Product>) UksRecNew(int Deadline, List<Resource> Resources, List<Product> Products, int idx)
        {
            // Base Case: If no more products are left to consider
            //int start_time = Deadline;
            int remaining_time = Deadline;
            if (idx == 0)
            {
                //Check resource
                float maxProfit = 0;
                List<Product> selectedProducts = new();

                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();
                List<Resource> Resources_new = DeepCopyRes(Resources);

                //When it gets to the point, because of how recursion works, the value for deadline will be how much space has been filled up as it traces back, so here I should inverse it
                //Deadline = total_time - Deadline;

                while (Resources_new.FindIndex(x => x.Count < 0) == -1 && remaining_time >= 0)
                {
                    remaining_time -= Products[0].Time_Cost;
                    for (int j = 0; j < resources_in_product.Length; j++)
                    {
                        int matching_resource_index = Resources_new.FindIndex(x => x.Name == resources_in_product[j]);


                        if (matching_resource_index != -1)
                        {
                            Resources_new[matching_resource_index].Count -= Int32.Parse(cost_of_each[j]);

                            //If the cost is greater than the count of that resource, no more of that item will be placed in the bag.
                            //Force return notTakeProfit
                            if (Resources_new[matching_resource_index].Count < 0 || remaining_time < 0)
                            {
                                Console.WriteLine(selectedProducts);
                                return (maxProfit, selectedProducts);
                            }
                        }
                    }
                    selectedProducts.Add((new Product
                    {
                        Id = selectedProducts.Count + 1,
                        Name = Products[0].Name,
                        Profit = Products[0].Profit,
                        Time_Cost = Products[0].Time_Cost,
                        ResourceNames = Products[0].ResourceNames,
                        ResourceCounts = Products[0].ResourceCounts

                    }));
                    maxProfit += Products[0].Profit;
                    Resources = Resources_new; //Try: Since this is the bottom of the call, it should be safe to modify Resources
                }
                Console.WriteLine(selectedProducts);
                return (maxProfit, selectedProducts);
            }
            //Check to see if has passed the resource limit. Return float.min so that it skips this branch

            // Recursive Case: Do not take the current product
            int not_take_time = Deadline;
            int take_time = Deadline;
            var (notTakeProfit, notTakeProducts) = UksRecNew(not_take_time, DeepCopyRes(Resources), Products, idx - 1);
            Console.Write(notTakeProducts);

            // Recursive Case: Consider taking the current product
            float takeProfit = float.MinValue;
            List<Product> takeProducts = new();
            if (Products[idx].Time_Cost <= Deadline)
            {
                //Subtract resources. Recursion call will check if there are negative counts
                List<Resource> Resources_Updated = DeepCopyRes(Resources);
                string[] resources_in_product = Products[idx].ResourceNames.Split(',').ToArray();
                string[] cost_of_each = Products[idx].ResourceCounts.Split(',').ToArray();
                bool canTake = true;

                for (int j = 0; j < resources_in_product.Length; j++)
                {
                    int matching_resource_index = Resources_Updated.FindIndex(x => x.Name == resources_in_product[j]);

                    if (matching_resource_index != -1)
                    {
                        Resources_Updated[matching_resource_index].Count -= Int32.Parse(cost_of_each[j]);
                        if (Resources_Updated[matching_resource_index].Count < 0)
                        {
                            canTake = false; break;
                        }
                    }
                        
                }
                if (!canTake)
                {
                    var(nextTakeProfit, nextTakeProducts) = (0, new List<Product>()); //May give an error if other branch contains a negative value 
                }
                else
                {
                    var (nextTakeProfit, nextTakeProducts) = UksRecNew(take_time - Products[idx].Time_Cost, Resources_Updated, Products, idx);
                    takeProfit = Products[idx].Profit + nextTakeProfit;
                    takeProducts = nextTakeProducts.Select(r => new Product
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Profit = r.Profit,
                        Time_Cost = r.Time_Cost,
                        ResourceNames = r.ResourceNames,
                        ResourceCounts = r.ResourceCounts
                    }).ToList();
                    takeProducts.Add(new Product
                    {
                        Id = Products[idx].Id,
                        Name = Products[idx].Name,
                        Profit = Products[idx].Profit,
                        Time_Cost = Products[idx].Time_Cost,
                        ResourceNames = Products[idx].ResourceNames,
                        ResourceCounts = Products[idx].ResourceCounts
                    });
                }

                Console.WriteLine(takeProducts);
                //Products[idx]); // Add the current product to the selected products

            }

            // Compare take vs not-take scenarios and return the better one
            if (takeProfit > notTakeProfit)
            {
                return (takeProfit, takeProducts);
            }
            else
            {
                return (notTakeProfit, notTakeProducts);
            }
        }
        //GET: /KnapSack
        public async Task<IActionResult> Index()
       // public List<Object[]> Index()
        {
            int time_limit = 100;
            if (ModelState.IsValid)
            {
                //Must sort list to ensure it returns the optimal solution, as there may be more than one valid solution
                List<Product> ProdList = await _context.Product.OrderByDescending(p => p.Profit / p.Time_Cost).ToListAsync();
                List<Resource> ResList = await _context.Resource.ToListAsync();
                

                var (max_profit, selectedProducts) = UksRecNew(time_limit, ResList, ProdList, ProdList.Count - 1);
                ViewBag.MaxProfit = max_profit;
                ViewBag.Products = selectedProducts;
                return View();
			}

			return View();

            //return View(); //TODO: Make the view
        }

    }
}
