using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class EducationStreamRepository : Repository<EducationStream>, IEducationStreamRepository
{
    public EducationStreamRepository(ApplicationDbContext context) : base(context)
    {
    }
}