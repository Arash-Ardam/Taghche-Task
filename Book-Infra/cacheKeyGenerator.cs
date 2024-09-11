using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Infra
{
    public static class cacheKeyGenerator
    {

        public static string GenerateKeyFromHttpContext(HttpContext httpContext)
        {
            string key = string.Empty;

            if (httpContext.Request.Method == HttpMethod.Get.ToString())
            {
                var path = httpContext.Request.Path.Value;
                if (path.Contains("api/book")) 
                {
                    key = $"BookId:{path.Last()}";
                }
            }
            return key ;
        }

        public static string GenerateKeyFromId(int id)
        {
            string key = $"BookId:{id}";
            return key ;
        }

    }
}
