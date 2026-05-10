using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.DTOs.Search;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Queries.Search;

public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SearchResultDto> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query?.ToLower() ?? "";

        var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);
        var filteredCourses = courses
            .Where(c => !c.IsDeleted && !c.IsDisabled)
            .Where(c => string.IsNullOrEmpty(query) || c.Title.ToLower().Contains(query) || (c.Description?.ToLower().Contains(query) ?? false) || (c.Category?.ToLower().Contains(query) ?? false))
            .Select(c => new CourseSearchResultDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Category = c.Category,
                Level = c.Level,
                InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                EnrollmentCount = c.Enrollments.Count,
                CreatedAt = c.CreatedAt
            })
            .Take(request.PageSize)
            .ToList();

        var lectures = await _unitOfWork.Lectures.GetAllAsync(cancellationToken);
        var filteredLectures = lectures
            .Where(l => !l.IsDeleted && !l.IsDisabled)
            .Where(l => string.IsNullOrEmpty(query) || l.Title.ToLower().Contains(query) || (l.Description?.ToLower().Contains(query) ?? false))
            .Select(l => new LectureSearchResultDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                CourseTitle = l.Course?.Title ?? "",
                OrderIndex = l.OrderIndex,
                DurationMinutes = l.DurationMinutes,
                IsPreview = l.IsPreview,
                CreatedAt = l.CreatedAt
            })
            .Take(request.PageSize)
            .ToList();

        var exams = await _unitOfWork.Exams.GetAllAsync(cancellationToken);
        var filteredExams = exams
            .Where(e => !e.IsDeleted)
            .Where(e => string.IsNullOrEmpty(query) || e.Title.ToLower().Contains(query) || (e.Description?.ToLower().Contains(query) ?? false))
            .Select(e => new ExamSearchResultDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                CourseTitle = e.Course?.Title,
                Type = e.Type,
                IsPublished = e.IsPublished,
                QuestionCount = e.Questions.Count,
                CreatedAt = e.CreatedAt
            })
            .Take(request.PageSize)
            .ToList();

        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var filteredStudents = users
            .Where(u => !u.IsDeleted && u.Role == Domain.Enums.UserRole.Student)
            .Where(u => string.IsNullOrEmpty(query) || u.FullName.ToLower().Contains(query) || u.PhoneNumber.ToLower().Contains(query) || u.Username.ToLower().Contains(query))
            .Select(u => new StudentSearchResultDto
            {
                Id = u.Id,
                Name = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Username = u.Username,
                EnrollmentCount = u.Enrollments.Count,
                LastLoginAt = u.LastLoginAt,
                IsActive = u.IsActive && !u.IsBanned
            })
            .Take(request.PageSize)
            .ToList();

        return new SearchResultDto
        {
            Courses = filteredCourses,
            Lectures = filteredLectures,
            Exams = filteredExams,
            Students = filteredStudents,
            TotalCount = filteredCourses.Count + filteredLectures.Count + filteredExams.Count + filteredStudents.Count
        };
    }
}

public class SearchCoursesQueryHandler : IRequestHandler<SearchCoursesQuery, PagedResult<SearchResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchCoursesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SearchResultDto>> Handle(SearchCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query?.ToLower() ?? "";
        var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);

        var filtered = courses
            .Where(c => !c.IsDeleted && !c.IsDisabled)
            .Where(c => string.IsNullOrEmpty(query) || c.Title.ToLower().Contains(query) || (c.Description?.ToLower().Contains(query) ?? false))
            .Where(c => string.IsNullOrEmpty(request.Category) || c.Category == request.Category)
            .Where(c => !request.Level.HasValue || c.Level == request.Level.Value)
            .Select(c => new SearchResultDto
            {
                Courses = new List<CourseSearchResultDto>
                {
                    new()
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        Category = c.Category,
                        Level = c.Level,
                        InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                        EnrollmentCount = c.Enrollments.Count,
                        CreatedAt = c.CreatedAt
                    }
                }
            })
            .ToList();

        var total = filtered.Count;
        var paged = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<SearchResultDto>
        {
            Items = paged,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
