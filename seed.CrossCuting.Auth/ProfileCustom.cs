﻿using Common.API.Extensions;
using Common.Domain.Enums;
using Common.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Seed.CrossCuting
{
    public static class ProfileCustom
    {

        enum ETypeRole
        {
            Admin = 1,
            Tenant = 2,
            Owner = 3,
        }

        enum ERole
        {
            Contributor,
            Reader
        }
		

		public static IDictionary<string, object> Define(IEnumerable<Claim> _claims)
        {
            var user = new CurrentUser().Init(Guid.NewGuid().ToString(), _claims.ConvertToDictionary());
            return Define(user);
        }

        public static IDictionary<string, object> Define(CurrentUser user)
        {
            var _claims = user.GetClaims();
            var roles = JsonConvert.DeserializeObject<IEnumerable<string>>(user.GetRole());
            var typeTole = user.GetTypeRole();

            if (typeTole.ToLower() == ETypeRole.Admin.ToString().ToLower())
                _claims.AddRange(ClaimsForAdmin());
            else
                _claims.AddRange(ClaimsForTenant(user.GetSubjectId<int>()));

            return _claims;
        }

        public static Dictionary<string, object> ClaimsForAdmin()
        {
            var tools = new List<dynamic>
            {
                new Tool { Icon = "fa fa-edit", Name = "Sample", Route = "/sample", Key = "Sample" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "SampleType", Route = "/sampletype", Key = "SampleType" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "SampleDash", Route = "/sampledash", Key = "SampleDash" , Type = ETypeTools.Menu },

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
                new Tool { Icon = "fa fa-edit", Name = "Sample", Route = "/sample", Key = "Sample" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "SampleType", Route = "/sampletype", Key = "SampleType" , Type = ETypeTools.Menu },
                new Tool { Icon = "fa fa-edit", Name = "SampleDash", Route = "/sampledash", Key = "SampleDash" , Type = ETypeTools.Menu },

            };

            var _toolsForSubscriber = JsonConvert.SerializeObject(tools);
            return new Dictionary<string, object>
            {
                { "tools", _toolsForSubscriber }
            };
        }

    }
}
