using Common.API;
using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace Seed.Api.Controllers
{
	
    [Route("api/[controller]")]
    public class AddCorsController : Controller
    {
        private readonly ICorsPolicyAccessor _corsPolicyAccessor;
        public AddCorsController(ICorsPolicyAccessor corsPolicyAccessor)
        {
            this._corsPolicyAccessor = corsPolicyAccessor;
        }

        [HttpGet]
        public string Get(string origin)
        {
            this._corsPolicyAccessor.GetPolicy("AllowStackOrigin").Origins.Add(origin);
            return "origin add success!";
        }
    }
}
