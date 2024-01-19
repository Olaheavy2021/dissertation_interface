
namespace Shared.DTO;

public class MatchingStatusRootObject
{
    public Dictionary<string, MatchingStatusStudentInfo> Result { get; set; } = null!;
    public string State { get; set; } = string.Empty;
}