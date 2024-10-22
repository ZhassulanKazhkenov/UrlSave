namespace UrlSave.Domain.Entities;

public class User : BaseEntity
{
    public User(string email)
    {
        Email = email;
    }

    public string Email {  get; set; }
}
