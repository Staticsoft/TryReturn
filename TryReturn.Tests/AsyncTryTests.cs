using System;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.TryReturn.Tests
{
    public class AsyncTryTests
    {
        [Fact]
        public async Task TryGetExpressionResult()
        {
            var result = await Try.Return(Greet).Result();
            Assert.Equal("Welcome to AsyncTry documentation!", result);
        }

        [Fact]
        public async Task TryDivideByZeroException()
        {
            await Assert.ThrowsAsync<DivideByZeroException>(() => Try.Return(ThrowDivideByZeroException).Result());
        }

        [Fact]
        public async Task TryHandleException()
        {
            var exception = await Assert.ThrowsAsync<CustomException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .Result()
            );
            Assert.IsType<DivideByZeroException>(exception.InnerException);
        }

        [Fact]
        public async Task CanRegisterMultipleHandlers()
        {
            await Assert.ThrowsAsync<CustomException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .On<Exception>(exception => exception)
                    .Result()
            );
        }

        [Fact]
        public async Task RespectsExceptionHandlingOrder()
        {
            await Assert.ThrowsAsync<DivideByZeroException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<Exception>(exception => exception)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .Result()
            );
        }

        static Task<int> ThrowDivideByZeroException()
        {
            var zero = 0;
            return Task.FromResult(1 / zero);
        }

        static Task<string> Greet()
            => Task.FromResult("Welcome to AsyncTry documentation!");
    }
}
