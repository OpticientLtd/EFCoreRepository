using System;

namespace Opticient.EFCore.Repository.Interfaces.Repositories;

/// <summary>
/// Represents the base contract for all repository classes in the system.
/// </summary>
/// <remarks>
/// The <see cref="IBaseRepository" /> interface serves as a foundational interface for defining
/// common repository behaviors, ensuring consistency across all repositories.
/// </remarks>
public interface IBaseRepository : IDisposable
{
}
