using MediatR;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Common;

/// <summary>
/// Implemented by any entity that raises domain events.
/// AppDbContext collects events from all tracked entities after
/// SaveChangesAsync and publishes them via MediatR.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyList<INotification> DomainEvents { get; }
    void AddDomainEvent(INotification domainEvent);
    void ClearDomainEvents();
}

/// <summary>
/// Mixin base class that entities can inherit alongside AuditableEntity
/// to gain domain event support without repeating the list management.
/// </summary>
public abstract class AggregateRoot : AuditableEntity, IHasDomainEvents
{
    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyList<INotification> DomainEvents
        => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
