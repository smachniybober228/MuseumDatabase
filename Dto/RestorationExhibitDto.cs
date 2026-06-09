public class RestorationExhibitDto
{
    public string ExhibitTitle { get; set; }
    public string OrderNumber { get; set; }
    public DateTime ReceiptDate { get; set; }
    public DateTime PlannedCompletionDate { get; set; }
    public string RestorerName { get; set; }
    public string Status { get; set; } // "В работе" или "Завершён, ожидает возврата"
}