using MediatR;
using PixelAcademy.Application.DTOs.Admin;
using PixelAcademy.Application.DTOs.AuditLogs;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Queries.Admin;

public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>;
public record GetAuditLogsQuery(int Page = 1, int PageSize = 50, AuditActionType? ActionType = null) : IRequest<PagedResult<AuditLogDto>>;
public record GetAdminAuditLogsQuery(int Page = 1, int PageSize = 50) : IRequest<PagedResult<AuditLogDto>>;
