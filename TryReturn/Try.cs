using Staticsoft.TryReturn;
using System.Threading.Tasks;

namespace System
{
    public static class Try
    {
        public static TryExpression<T> Return<T>(Func<T> func)
            => new(func);

        public static AsyncTryExpression<T> Return<T>(Func<Task<T>> func)
            => new(func);
    }
}
