using System;
using System.Collections.Generic;
using System.Linq;

namespace Staticsoft.TryReturn
{
    public class TryExpression<TExpression>
    {
        readonly Func<TExpression> Func;
        readonly IEnumerable<ExceptionHandler> Handlers;

        public TryExpression(Func<TExpression> func)
            : this(func, Enumerable.Empty<ExceptionHandler>()) { }

        TryExpression(Func<TExpression> func, IEnumerable<ExceptionHandler> handlers)
            => (Func, Handlers) = (func, handlers);

        public TExpression Result()
        {
            try
            {
                return Func.Invoke();
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

        public TryExpression<TExpression> On<T>(Func<T, Exception> handler) where T : Exception
            => On<TExpression>(ex => handler(ex as T), typeof(T));

        TryExpression<TExpression> On<T>(Func<Exception, Exception> handler, Type exceptionType)
            => new(Func, Handlers.Append(new ExceptionHandler(handler, exceptionType)));
    }
}
