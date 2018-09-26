using Common.API.Extensions;
using Common.Domain.Enums;
using Common.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Score.Platform.Account.CrossCuting
{
    public static class ProfileCustom
    {

        enum Role
        {
            Admin = 1,
            Tenant = 2,
            Owner = 3,
        }
		

		public static IDictionary<string, object> Define(IEnumerable<Claim> _claims)
        {
            var user = new CurrentUser().Init(Guid.NewGuid().ToString(), _claims.ConvertToDictionary());
            return Define(user);
        }

        public static IDictionary<string, object> Define(CurrentUser user)
        {
            var _claims = user.GetClaims();
            var role = GetRole(_claims);
            var typeTole = GetTypeRole(_claims);

            if (role.ToLower() == Role.Admin.ToString().ToLower())
                _claims.AddRange(ClaimsForAdmin());
            else
            {
                _claims.AddRange(ClaimsForTenant(user.GetSubjectId<int>()));
            }

            return _claims;
        }

        private static string GetRole(IDictionary<string, object> _claims)
        {
            var role = _claims.Where(_ => _.Key == "role").SingleOrDefault();
            return role.Value.IsNotNull() ? role.Value.ToString() : "--";
        }

        private static string GetTypeRole(IDictionary<string, object> _claims)
        {
            var typeRole = _claims.Where(_ => _.Key == "typerole");
            if (typeRole.IsAny())
                return typeRole.SingleOrDefault().Value.ToString();
            return string.Empty;
        }

        public static Dictionary<string, object> ClaimsForAdmin()
        {
            var tools = new List<dynamic>
            {
                new Tool { Icon = "fa fa-edit", Name = "Program", Route = "/program", Key = "Program" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "Tenant", Route = "/tenant", Key = "Tenant" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "Thema", Route = "/thema", Key = "Thema" , Type = ETypeTools.Menu },

            };
            var _toolsForAdmin = JsonConvert.SerializeObject(tools);
            return new Dictionary<string, object>
            {
                { "tools", _toolsForAdmin }
            };
        }

        public static Dictionary<string, object> ClaimsForTenant(int tenantId)
        {

            var tools = new List<Tool>
            {
                new Tool { Icon = "fa fa-edit", Name = "Tool", Route = "#/Url", Key="Tool" },
            };

            var _toolsForSubscriber = JsonConvert.SerializeObject(tools);
            return new Dictionary<string, object>
            {
                { "tools", _toolsForSubscriber }
            };
        }

    }
}