namespace UrlSave.Entities;

public class Product : BaseEntity
{
    public Product(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Price> Prices { get; set; }
}
