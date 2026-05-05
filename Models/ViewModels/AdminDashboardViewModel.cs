namespace NextInLine.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsersCount   { get; set; }
    public int TicketsToday      { get; set; }
    public int CompletedToday    { get; set; }
    public string AvgWaitTime    { get; set; } = "0 min";
}
