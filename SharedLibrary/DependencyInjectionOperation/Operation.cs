using System;

namespace CoreLibrary.DependencyInjectionOperation
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0

    public class Operation : IOperationTransient, IOperationScoped, IOperationSingleton
    {
        public Operation()
        {
            OperationId = Guid.NewGuid().ToString()[^4..];
        }

        public string OperationId { get; }
    }
}
