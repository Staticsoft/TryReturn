using System;

namespace Staticsoft.TryReturn
{
    public class ExceptionHandler
    {
        readonly Func<Exception, Exception> Handler;
        public readonly Type ExceptionType;

        public ExceptionHandler(Func<Exception, Exception> handler, Type exceptionType)
            => (Handler, ExceptionType) = (handler, exceptionType);

        public Exception Handle(Exception exception)
            => Handler(exception);
    }
}
