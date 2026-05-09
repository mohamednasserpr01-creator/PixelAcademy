using System;

namespace PixelAcademy.Domain.Common;

public abstract class BaseEntity<TId> where TId : struct
{
    public TId Id { get; set; }
}

public abstract class BaseEntity : BaseEntity<Guid>
{
}
