namespace TicketManagement.Models.DTO
{
    public class OrderDTO
    {
        public int EventID { get; set; }
        public DateTime OrderedAt { get; set; }
        public int TicketCategoryID { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
