namespace CRUD_PRACT.Middlewares
{
    public class BlockToken
    {

        private readonly RequestDelegate next;
        public BlockToken(RequestDelegate next)
        {
            this.next = next;
        }
       private static HashSet<string> tokens = new HashSet<string>();

        public static void addBlacklist(string token)
        {
           
            
                tokens.Add(token);
          
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(token != null && tokens.Contains(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            await next(context);
        }

        public static bool IsBlacklisted(string token)
        {
            if (token != null && tokens.Contains(token))
            {
                return true;
            }
            return false;
        }
    }
    
}
