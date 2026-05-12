using MediatR;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.Teachers;

// --- 1. إضافة مدرس ---
public class CreateTeacherCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateTeacherCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = new User
        {
            FullName = request.Name,
            PhoneNumber = request.Phone,
            Username = request.Phone, // بنخلي اليوزرنيم هو الرقم مبدئياً
            PasswordHash = request.Password, // ⚠️ ملاحظة: المفروض تشفر الباسورد هنا بالـ Hashing Service بتاعتك
            Role = UserRole.Teacher, // تأكد إن Teacher موجودة في الـ Enum بتاعك
            IsActive = true,
            IsBanned = false
        };

        await _unitOfWork.Users.AddAsync(teacher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return teacher.Id;
    }
}

// --- 2. تعديل مدرس ---
public class UpdateTeacherCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Password { get; set; }
}

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateTeacherCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (teacher == null || teacher.Role != UserRole.Teacher) return false;

        teacher.FullName = request.Name;
        teacher.PhoneNumber = request.Phone;
        
        if (!string.IsNullOrEmpty(request.Password))
            teacher.PasswordHash = request.Password; // ⚠️ يرجى التشفير بردو لو اتغيرت

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// --- 3. حظر / فك حظر مدرس ---
public class ToggleTeacherBanCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public bool Ban { get; set; } // True = حظر, False = فك الحظر
}

public class ToggleTeacherBanCommandHandler : IRequestHandler<ToggleTeacherBanCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public ToggleTeacherBanCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(ToggleTeacherBanCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (teacher == null || teacher.Role != UserRole.Teacher) return false;

        teacher.IsBanned = request.Ban;
        teacher.BannedAt = request.Ban ? DateTime.UtcNow : null;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// --- 4. حذف مدرس ---
public class DeleteTeacherCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTeacherCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (teacher == null || teacher.Role != UserRole.Teacher) return false;

        await _unitOfWork.Users.DeleteAsync(teacher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}