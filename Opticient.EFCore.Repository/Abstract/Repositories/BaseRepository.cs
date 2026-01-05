using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Interfaces.Repositories;

using System;

namespace Opticient.EFCore.Repository.Abstract.Repositories;

/// <inheritdoc />
public abstract class BaseRepository(DbContext dbContext, ILogger<BaseRepository> logger) : IBaseRepository
{
    protected readonly ILogger<BaseRepository> Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected readonly DbContext DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                dbContext.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}