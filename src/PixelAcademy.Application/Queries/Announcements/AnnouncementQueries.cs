using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Announcements;
using PixelAcademy.Application.DTOs.Common;

namespace PixelAcademy.Application.Queries.Announcements;

public record GetAnnouncementsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<AnnouncementDto>>;
public record GetAnnouncementByIdQuery(Guid Id) : IRequest<AnnouncementDto?>;
public record GetCourseAnnouncementsQuery(Guid CourseId) : IRequest<IReadOnlyList<AnnouncementDto>>;
