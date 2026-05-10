using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Users.IsPhoneNumberUniqueAsync(request.PhoneNumber, cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            PhoneNumber = request.PhoneNumber,
            Username = request.PhoneNumber, // حفظنا رقم التليفون كـ Username أوتوماتيك
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            ParentPhoneNumber = request.ParentPhoneNumber,
            Governorate = request.Governorate,
            Address = request.Address,
            SchoolName = request.SchoolName,
            EducationalStageId = request.EducationalStageId,
            EducationStreamId = request.EducationStreamId,
            CreatedAt = _dateTimeProvider.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var accessTokenExpiresAt = _jwtService.GetAccessTokenExpiration();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = _dateTimeProvider.UtcNow.AddDays(7),
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }
}