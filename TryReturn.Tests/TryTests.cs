using System;
using Xunit;

namespace Staticsoft.TryReturn.Tests
{
    public class TryTests
    {
        [Fact]
        public void TryDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => Try.Return(ThrowDivideByZeroException).Result());
        }

        [Fact]
        public void TryGetExpressionResult()
        {
            var result = Try.Return(Greet).Result();
            Assert.Equal("Welcome to Try documentation!", result);
        }

        [Fact]
        public void TryHandleException()
        {
            var exception = Assert.Throws<CustomException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .Result()
            );
            Assert.IsType<DivideByZeroException>(exception.InnerException);
        }

        [Fact]
        public void CanRegisterMultipleHandlers()
        {
            Assert.Throws<CustomException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .On<Exception>(exception => exception)
                    .Result()
            );
        }

        [Fact]
        public void RespectsExceptionHandlingOrder()
        {
            Assert.Throws<DivideByZeroException>(() =>
                Try.Return(ThrowDivideByZeroException)
                    .On<Exception>(exception => exception)
                    .On<DivideByZeroException>(exception => new CustomException(exception))
                    .Result()
            );
        }

        static string Greet()
            => "Welcome to Try documentation!";

        static int ThrowDivideByZeroException()
        {
            var zero = 0;
            return 1 / zero;
        }
    }

    public class CustomException : Exception
    {
        public CustomException(DivideByZeroException inner) : base(nameof(CustomException), inner) { }
    }
}
