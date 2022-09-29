namespace MVCcrypto.Models.Models;

public class Currency : BaseEntity
{
    public string Name { get; set; } = "";
    public double CurrentPrice { get; set; } = 0.0;
    public DateTime AddedDate { get; set; } 
}