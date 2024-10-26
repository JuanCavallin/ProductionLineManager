namespace InventoryTrackingApp.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public Resource() 
        { 
        
        }

        public Resource(int id, string name, int count)
        {
            Id = id;
            Name = name;
            Count = count;
        }

        public Resource(string name, int count) { //Might give an error if Id is not instantiated
            Name = name;
            Count = count;

        }
    }
}
