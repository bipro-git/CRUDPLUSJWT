
using Azure.Core;

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
            var response = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;



            var request = context.Request;
            var logDetails = "\n\n\n[Request]\n" + 
                         $"Timestamp: {DateTime.Now}\n" +
                         $"Method: {request.Method}\n" +
                         $"Path: {request.Path}\n" +
                         $"QueryString: {request.QueryString.ToString()}\n" +
                         $"Headers: {request.Headers}\n" +
                         $"Body : {request.Body}\n"+
                         $"Ip Address : {context.Connection.RemoteIpAddress.ToString()}";


            await File.AppendAllTextAsync("MiddlewareLog.txt", logDetails);
            await next(context);
            await LogResponseAsync(context);
            await responseBodyStream.CopyToAsync(response);

        }

        private async Task LogResponseAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var logDetails = "\n\n[Response]\n"+
                             $"Timestamp: {DateTime.Now}\n" +
                             $"StatusCode: {context.Response.StatusCode}\n" +
                             $"Response: {responseText}\n\n";

            await File.AppendAllTextAsync("MiddlewareLog.txt", logDetails);

        }
    }
}
