using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class EducationalStageRepository : Repository<EducationalStage>, IEducationalStageRepository
{
    public EducationalStageRepository(ApplicationDbContext context) : base(context)
    {
    }
}