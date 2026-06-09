public class RestorationHistoryDto
{
    public string OrderNumber { get; set; }
    public DateTime ReceiptDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string FinalReport { get; set; }
    public double TotalCost { get; set; }
    public List<string> WorkLogDescriptions { get; set; }
}