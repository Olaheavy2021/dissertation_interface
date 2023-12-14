namespace Dissertation.Application.DTO.Response;

public class AdminMetricsResponse
{
    public int Students { get; set; }

    public int Supervisors { get; set; }

    public int ApprovedRequests { get; set; }

    public int DeclinedRequests { get; set; }
}