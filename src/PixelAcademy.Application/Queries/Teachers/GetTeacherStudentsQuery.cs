using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Queries.Teachers;

public class ExportStudentDto
{
    public string StudentName { get; set; } = string.Empty;
    public string StudentPhone { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
}

public class GetTeacherStudentsQuery : IRequest<List<ExportStudentDto>>
{
    public Guid TeacherId { get; set; }
    public string? Stage { get; set; } 
}

public class GetTeacherStudentsQueryHandler : IRequestHandler<GetTeacherStudentsQuery, List<ExportStudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTeacherStudentsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<ExportStudentDto>> Handle(GetTeacherStudentsQuery request, CancellationToken cancellationToken)
    {
        var studentsList = new List<ExportStudentDto>();

        // =========================================================================
        // 1️⃣ هنجيب الطلاب اللي مشتركين في (الكورسات، العروض، بنك المعرفة)
        // =========================================================================
        var enrollments = await _unitOfWork.Enrollments.AsQueryable()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.Course != null && e.Course.InstructorId == request.TeacherId)
            .ToListAsync(cancellationToken);

        foreach (var e in enrollments)
        {
            if (e.Student == null) continue;

            // 🚀 بنقرأ من الـ Level اللي موجود بالفعل جوه ملف Course.cs
            var stageName = e.Course?.Level.ToString() ?? "غير محدد"; 

            // فلترة لو الأدمن اختار صف معين من القائمة
            if (!string.IsNullOrEmpty(request.Stage) && request.Stage != "الكل" && stageName != request.Stage)
                continue;

            studentsList.Add(new ExportStudentDto
            {
                StudentName = e.Student.FullName ?? e.Student.Username,
                StudentPhone = e.Student.PhoneNumber ?? "غير مسجل",
                ParentPhone = e.Student.ParentPhoneNumber ?? "غير مسجل",
                Stage = stageName,
                CourseName = e.Course?.Title ?? "اشتراك عام",
                EnrollmentDate = e.CreatedAt
            });
        }

        // =========================================================================
        // 2️⃣ هنجيب الطلاب اللي اشتروا (محاضرات منفصلة) 
        // =========================================================================
        var lectureAccesses = await _unitOfWork.LectureAccesses.AsQueryable()
            .Include(la => la.Student)
            .Include(la => la.Lecture)
                .ThenInclude(l => l.Course)
            .Where(la => la.Lecture != null && la.Lecture.Course != null && la.Lecture.Course.InstructorId == request.TeacherId)
            .ToListAsync(cancellationToken);

        foreach (var la in lectureAccesses)
        {
            if (la.Student == null) continue;

            // 🚀 بنقرأ من الـ Level اللي موجود بالفعل جوه الكورس المرتبط بالمحاضرة
            var stageName = la.Lecture?.Course?.Level.ToString() ?? "غير محدد";

            if (!string.IsNullOrEmpty(request.Stage) && request.Stage != "الكل" && stageName != request.Stage)
                continue;

            studentsList.Add(new ExportStudentDto
            {
                StudentName = la.Student.FullName ?? la.Student.Username,
                StudentPhone = la.Student.PhoneNumber ?? "غير مسجل",
                ParentPhone = la.Student.ParentPhoneNumber ?? "غير مسجل",
                Stage = stageName,
                CourseName = $"محاضرة: {la.Lecture?.Title}", 
                EnrollmentDate = la.CreatedAt 
            });
        }

        // =========================================================================
        // 3️⃣ تصفية الداتا (مسح التكرار)
        // =========================================================================
        var uniqueStudents = studentsList
            .GroupBy(s => new { s.StudentPhone, s.CourseName })
            .Select(g => g.First())
            .OrderByDescending(s => s.EnrollmentDate)
            .ToList();

        return uniqueStudents;
    }
}