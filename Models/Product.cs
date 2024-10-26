using Microsoft.VisualBasic;

namespace InventoryTrackingApp.Models
{
    public class Product
    {
        //Product = {0: Id, 1: Name, 2: Profit, 3: time, 4: resources(index)}
        //Object[] Product1 = new Object[] { 0, "iPad", 2, 3, new int[] { 0, 1, 2 } };
        public int Id { get; set; }
        public string Name { get; set; }
        public float Profit { get; set; }
        public int Time_Cost { get; set; }

        public string ResourceNames { get; set; }
        public string ResourceCounts { get; set; }

// public int Cost { get; set; }



        public Product() 
        { 
               
        }

        public List<Resource> ReturnUsedResources()
        {
            List<string> names = ResourceNames.Split(',').ToList<string>();
            List<string> counts = ResourceCounts.Split(',').ToList<string>();

			List<Resource> Resources =  new List<Resource>() { new Resource(names[0], Int32.Parse(counts[0])) }; //Resets the list

            for (int i = 1; i < names.Count; i++)
            {
                Resources.Add(new Resource(names[i], Int32.Parse(counts[i])));
            }

            return Resources;
        }
    }
}
