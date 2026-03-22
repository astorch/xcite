using System;

namespace xcite.logging;

/// <summary>
/// Gives control over the lifecycle if a log context property.
/// When this instance is disposed, the related context property is removed from the context. 
/// </summary>
public interface ILogContextProperty : IDisposable, IAsyncDisposable;