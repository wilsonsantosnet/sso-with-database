using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API
{
    public class CorsPolicyAccessor : ICorsPolicyAccessor
    {
        private readonly CorsOptions _options;

        public CorsPolicyAccessor(IOptions<CorsOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        public CorsPolicy GetPolicy()
        {
            return _options.GetPolicy(_options.DefaultPolicyName);
        }

        public CorsPolicy GetPolicy(string name)
        {
            return _options.GetPolicy(name);
        }
    }
}
