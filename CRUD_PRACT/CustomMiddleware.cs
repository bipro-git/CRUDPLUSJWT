
namespace CRUD_PRACT
{
    public class CustomMiddleware 
    {

        private readonly RequestDelegate next;

        public CustomMiddleware(RequestDelegate next)
        {
            this.next = next;   
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var logDetails = $"Timestamp: {DateTime.Now}\n" +
                         $"Method: {request.Method}\n" +
                         $"Path: {request.Path}\n" +
                         $"QueryString: {request.QueryString}\n" +
                         $"Headers: {request.Headers}\n";

            await File.AppendAllTextAsync("RequestLog.txt", logDetails);
            await next(context);

        }
    }
}
