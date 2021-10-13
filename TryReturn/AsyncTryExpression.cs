using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TryReturn
{
    public class AsyncTryExpression<TTask>
    {
        readonly Task<TTask> Task;
        readonly IEnumerable<ExceptionHandler> Handlers;

        public AsyncTryExpression(Task<TTask> task)
            : this(task, Enumerable.Empty<ExceptionHandler>()) { }

        AsyncTryExpression(Task<TTask> task, IEnumerable<ExceptionHandler> handlers)
            => (Task, Handlers) = (task, handlers);

        public async Task<TTask> Result()
        {
            try
            {
                return await Task;
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
            => new(Task, Handlers.Append(new ExceptionHandler(handler, exceptionType)));
    }
}
