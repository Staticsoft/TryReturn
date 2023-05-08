using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TryReturn;

public class AsyncTryExpression<TTask>
{
    readonly Func<Task<TTask>> Func;
    readonly IEnumerable<ExceptionHandler> Handlers;

    public AsyncTryExpression(Func<Task<TTask>> func)
        : this(func, Enumerable.Empty<ExceptionHandler>()) { }

    AsyncTryExpression(Func<Task<TTask>> func, IEnumerable<ExceptionHandler> handlers)
        => (Func, Handlers) = (func, handlers);

    public async Task<TTask> Result()
    {
        try
        {
            return await Func();
        }
        catch (Exception exception)
        {
            var exceptionType = exception.GetType();
            foreach (var handler in Handlers)
            {
                if (handler.ExceptionType.IsAssignableFrom(exceptionType))
                {
                    throw handler.Handle(exception);
                }
            }
            throw;
        }
    }

    public AsyncTryExpression<TTask> On<T>(Func<T, Exception> handler) where T : Exception
        => On<TTask>(ex => handler(ex as T), typeof(T));

    AsyncTryExpression<TTask> On<T>(Func<Exception, Exception> handler, Type exceptionType)
        => new(Func, Handlers.Append(new ExceptionHandler(handler, exceptionType)));
}
