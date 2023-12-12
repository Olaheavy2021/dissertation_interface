using Shared.DTO;
using Shared.Enums;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Helpers;

public static class CustomMappers
{
    public static UserListDto MapToUserDto(ApplicationUser applicationUser) =>
        new()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            IsLockedOut = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow,
            EmailConfirmed = applicationUser.EmailConfirmed,
            Status = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow
                ? UserStatus.Deactivated
                : applicationUser.EmailConfirmed ? UserStatus.Active : UserStatus.Inactive
        };

    public static StudentListDto MapToUserDto(ApplicationUser applicationUser, IEnumerable<GetCourse> courses) =>
        new()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            IsLockedOut = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow,
            EmailConfirmed = applicationUser.EmailConfirmed,
            Status = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow
                ? UserStatus.Deactivated
                : applicationUser.EmailConfirmed ? UserStatus.Active : UserStatus.Inactive,
            Course = courses.FirstOrDefault(x => x.Id == applicationUser.CourseId)! // Map courses
        };

    public static SupervisorListDto MapToUserDto(ApplicationUser applicationUser, IEnumerable<GetDepartment> departments) =>
        new()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            IsLockedOut = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow,
            EmailConfirmed = applicationUser.EmailConfirmed,
            Status = applicationUser.LockoutEnd >= DateTimeOffset.UtcNow
                ? UserStatus.Deactivated
                : applicationUser.EmailConfirmed ? UserStatus.Active : UserStatus.Inactive,
            Department = departments.FirstOrDefault(x => x.Id == applicationUser.DepartmentId)! // Map department
        };

    public static SupervisionRequestListDto MapToSupervisionRequestListDto(
        SupervisionRequest supervisionRequest, IEnumerable<GetDepartment> departments, IEnumerable<GetCourse> courses) =>
        new()
        {
            Id = supervisionRequest.Id,
            Status = supervisionRequest.Status,
            DissertationCohortId = supervisionRequest.DissertationCohortId,
            StudentDetails = MapToUserDto(supervisionRequest.Student, courses),
            SupervisorDetails = MapToUserDto(supervisionRequest.Supervisor, departments)
        };
    public static SupervisionListDto MapToSupervisionListDto(
        SupervisionList supervisionList, IEnumerable<GetDepartment> departments, IEnumerable<GetCourse> courses) =>
        new()
        {
            DissertationCohortId = supervisionList.DissertationCohortId,
            StudentDetails = MapToUserDto(supervisionList.Student, courses),
            SupervisorDetails = MapToUserDto(supervisionList.Supervisor, departments),
            Id = supervisionList.Id
        };
}