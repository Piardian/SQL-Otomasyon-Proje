namespace KutuphaneOtomasyon;

internal sealed class UserRewardSummary
{
    public int Score { get; set; }

    public decimal TotalPenalty { get; set; }

    public decimal UnpaidPenalty { get; set; }

    public int ActiveRentCount { get; set; }

    public int ReservationCount { get; set; }
}
