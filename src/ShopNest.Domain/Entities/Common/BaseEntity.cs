using System;

namespace ShopNest.Domain.Entities.Common;

public abstract class BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
}
