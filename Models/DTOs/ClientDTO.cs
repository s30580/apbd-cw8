using System.Globalization;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    public int ClientID { get; set; }
    public List<RegPay> List { get; set; }
    
}

public class RegPay
{
    public TripDTO Trip { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime? PaymentDate { get; set; }

   public static DateTime ParseDate(int dateInt)
    {
        string dateString = dateInt.ToString();
        int year = int.Parse(dateString.Substring(0, 4));
        int month = int.Parse(dateString.Substring(4, 2));
        int day = int.Parse(dateString.Substring(6, 2));

        return new DateTime(year, month, day);
    }
}