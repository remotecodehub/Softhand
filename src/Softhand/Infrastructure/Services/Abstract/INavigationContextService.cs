namespace Softhand.Infrastructure.Services.Abstract;

public interface INavigationContextService<T> where T : class
{
    T Payload { get; set; }
}
