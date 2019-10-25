using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Common.API
{
    public interface ICorsPolicyAccessor
    {
        CorsPolicy GetPolicy();
        CorsPolicy GetPolicy(string name);
    }


}