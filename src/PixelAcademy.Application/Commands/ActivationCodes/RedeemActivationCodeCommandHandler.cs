using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class RedeemActivationCodeCommandHandler : IRequestHandler<RedeemActivationCodeCommand, RedeemResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RedeemActivationCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RedeemResultDto> Handle(RedeemActivationCodeCommand request, CancellationToken cancellationToken)
    {
        var activationCode = await _unitOfWork.ActivationCodes.GetByCodeAsync(request.Code, cancellationToken);
        if (activationCode == null)
            throw new BadRequestException("Invalid activation code.");

        if (!activationCode.IsActive)
            throw new BadRequestException("This activation code has been disabled.");

        if (activationCode.IsFullyRedeemed)
            throw new BadRequestException("This activation code has reached its maximum redemptions.");

        if (activationCode.IsExpired)
            throw new BadRequestException("This activation code has expired.");

        var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null) throw new NotFoundException("User", request.StudentId);

        // Prevent duplicate personal redeem for non-wallet types
        if (activationCode.Type != CodeType.WalletCredit)
        {
            var existingPersonal = await HasPersonalRedemptionAsync(activationCode, request.StudentId, cancellationToken);
            if (existingPersonal)
                throw new ConflictException("You have already redeemed this code.");
        }

        // Increment redemption counter
        activationCode.CurrentRedemptions++;
        activationCode.LastRedeemedById = request.StudentId;
        activationCode.LastRedeemedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.ActivationCodes.UpdateAsync(activationCode, cancellationToken);

        var result = new RedeemResultDto
        {
            Success = true,
            Type = activationCode.Type,
            RemainingRedemptions = activationCode.MaxRedemptions - activationCode.CurrentRedemptions
        };

        switch (activationCode.Type)
        {
            case CodeType.WalletCredit:
                if (!activationCode.Value.HasValue)
                    throw new BadRequestException("Wallet credit code has no value.");
                
                var balanceBefore = student.WalletBalance;
                student.WalletBalance += activationCode.Value.Value;
                await _unitOfWork.Users.UpdateAsync(student, cancellationToken);
                
                result.NewWalletBalance = student.WalletBalance;
                result.Message = $"Added {activationCode.Value.Value} credits to your wallet.";
                
                await CreateWalletTransactionAsync(student.Id, WalletTransactionType.Recharge, activationCode.Value.Value,
                    balanceBefore, student.WalletBalance, $"Wallet recharge from code {activationCode.Code}", activationCode.Id, null, null, cancellationToken);
                break;

            case CodeType.CourseEnrollment:
                if (!activationCode.CourseId.HasValue)
                    throw new BadRequestException("Course enrollment code has no course assigned.");
                if (await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, activationCode.CourseId.Value, cancellationToken))
                    throw new ConflictException("You are already enrolled in this course.");
                
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = request.StudentId,
                    CourseId = activationCode.CourseId.Value,
                    Status = EnrollmentStatus.Active,
                    ProgressPercent = 0,
                    ActivationCodeId = activationCode.Id,
                    CreatedAt = _dateTimeProvider.UtcNow
                };
                await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
                
                result.CourseId = activationCode.CourseId.Value;
                result.Message = "Successfully enrolled in the course.";
                break;

            case CodeType.LectureAccess:
                if (!activationCode.LectureId.HasValue)
                    throw new BadRequestException("Lecture access code has no lecture assigned.");
                if (await _unitOfWork.LectureAccesses.HasAccessAsync(request.StudentId, activationCode.LectureId.Value, cancellationToken))
                    throw new ConflictException("You already have access to this lecture.");
                
                var lectureAccess = new LectureAccess
                {
                    Id = Guid.NewGuid(),
                    StudentId = request.StudentId,
                    LectureId = activationCode.LectureId.Value,
                    ActivationCodeId = activationCode.Id,
                    CreatedAt = _dateTimeProvider.UtcNow
                };
                await _unitOfWork.LectureAccesses.AddAsync(lectureAccess, cancellationToken);
                
                result.LectureId = activationCode.LectureId.Value;
                result.Message = "Lecture access granted.";
                break;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task<bool> HasPersonalRedemptionAsync(ActivationCode code, Guid studentId, CancellationToken cancellationToken)
    {
        // For CourseEnrollment: check if already enrolled via this code
        if (code.Type == CodeType.CourseEnrollment && code.CourseId.HasValue)
        {
            var existingEnrollment = await _unitOfWork.Enrollments.GetByStudentAndCourseAsync(studentId, code.CourseId.Value, cancellationToken);
            return existingEnrollment?.ActivationCodeId == code.Id;
        }
        // For LectureAccess: check if already has access via this code
        if (code.Type == CodeType.LectureAccess && code.LectureId.HasValue)
        {
            var existingAccess = await _unitOfWork.LectureAccesses.GetByStudentAndLectureAsync(studentId, code.LectureId.Value, cancellationToken);
            return existingAccess?.ActivationCodeId == code.Id;
        }
        return false;
    }

    private async Task CreateWalletTransactionAsync(Guid userId, WalletTransactionType type, decimal amount,
        decimal balanceBefore, decimal balanceAfter, string description, Guid? codeId, Guid? courseId, Guid? lectureId, CancellationToken cancellationToken)
    {
        var transaction = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = balanceAfter,
            Description = description,
            ActivationCodeId = codeId,
            CourseId = courseId,
            LectureId = lectureId,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.WalletTransactions.AddAsync(transaction, cancellationToken);
    }
}
