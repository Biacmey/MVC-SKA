namespace MvcSka.Models;

public class CustomerModel
{
    public CustomerModel(int id, string name, int balance)
    {
        this.id = id;
        this.name = name;
        this.balance = balance;
    }

    public int id { get;}
    public string name{ get;}
    public int balance{ get;}
}