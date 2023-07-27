namespace TicketManagement.Models.DTO
{
    public class OrderPatchDTO
    {
        public int OrderID { get; set; }
        public int EventID { get; set; }
        public string? TicketDescription { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
