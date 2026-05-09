using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Queries.Assignments;

public class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, IReadOnlyList<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssignmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Assignment> assignments;
        if (request.CourseId.HasValue)
            assignments = await _unitOfWork.Assignments.GetByCourseAsync(request.CourseId.Value, cancellationToken);
        else
            assignments = await _unitOfWork.Assignments.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<AssignmentDto>>(assignments);
    }
}

public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssignmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AssignmentDto?> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null) return null;
        return _mapper.Map<AssignmentDto>(assignment);
    }
}

public class GetAssignmentSubmissionsQueryHandler : IRequestHandler<GetAssignmentSubmissionsQuery, IReadOnlyList<AssignmentSubmissionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssignmentSubmissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AssignmentSubmissionDto>> Handle(GetAssignmentSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var submissions = await _unitOfWork.AssignmentSubmissions.GetByAssignmentAsync(request.AssignmentId, cancellationToken);
        return _mapper.Map<IReadOnlyList<AssignmentSubmissionDto>>(submissions);
    }
}

public class GetMySubmissionQueryHandler : IRequestHandler<GetMySubmissionQuery, AssignmentSubmissionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMySubmissionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AssignmentSubmissionDto?> Handle(GetMySubmissionQuery request, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.AssignmentSubmissions.GetByStudentAndAssignmentAsync(request.StudentId, request.AssignmentId, cancellationToken);
        if (submission == null) return null;
        return _mapper.Map<AssignmentSubmissionDto>(submission);
    }
}
