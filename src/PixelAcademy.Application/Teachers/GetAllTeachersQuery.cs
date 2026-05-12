using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Queries.Teachers;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // 🚀 ضفنا الباسورد هنا عشان يتبعت
    public string Status { get; set; } = string.Empty;
    public int CoursesCount { get; set; }
    public int StudentsCount { get; set; }
    public List<string> Subjects { get; set; } = new();
}

public class GetAllTeachersQuery : IRequest<List<TeacherDto>> { }

public class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, List<TeacherDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllTeachersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<TeacherDto>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _unitOfWork.Users.AsQueryable()
            .Where(u => u.Role == UserRole.Teacher)
            .Include(u => u.OwnedCourses)
                .ThenInclude(c => c.Enrollments)
            .ToListAsync(cancellationToken);

        return teachers.Select(t => new TeacherDto
        {
            Id = t.Id,
            Name = t.FullName ?? t.Username,
            Phone = t.PhoneNumber,
            Password = t.PasswordHash, // 🚀 هنا خلينا الكود يقرأ الباسورد ويبعته للفرونت إند
            Status = t.IsBanned ? "banned" : "accepted",
            CoursesCount = t.OwnedCourses?.Count ?? 0,
            // بنحسب عدد الطلاب بجمع الـ Enrollments بتاعة كل كورساته
            StudentsCount = t.OwnedCourses?.Sum(c => c.Enrollments?.Count ?? 0) ?? 0,
            Subjects = new List<string>() // المدرس هيضيفها بنفسه من لوحته بعدين
        }).ToList();
    }
}