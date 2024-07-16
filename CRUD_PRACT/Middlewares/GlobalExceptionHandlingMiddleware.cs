using System.Net;

namespace CRUD_PRACT.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;
        
        public GlobalExceptionHandlingMiddleware(RequestDelegate next , ILogger<GlobalExceptionHandlingMiddleware> logger )
        {
         this.next=next;
         this.logger = logger;
        }
        public  async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception e) {
                logger.LogError(e,e.Message);
                context.Response.StatusCode = (int) HttpStatusCode.SeeOther; 
            }
        }
    }
}
