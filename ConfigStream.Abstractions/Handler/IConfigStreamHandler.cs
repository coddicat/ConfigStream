using System.Threading.Tasks;

namespace ConfigStream.Abstractions.Handler
{
    public interface IConfigStreamHandler
    {
        Task HandleAsync(IHandlerContext context, HandlerDelegate next);
    }
}
