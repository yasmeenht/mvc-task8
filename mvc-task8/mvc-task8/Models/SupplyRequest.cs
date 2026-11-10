using Microsoft.AspNetCore.Identity;

namespace mvc_task8.Models
{
    public class SupplyRequest
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        public string UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;

        public List<RequestItem> RequestItems { get; set; } = new();
    }
}

