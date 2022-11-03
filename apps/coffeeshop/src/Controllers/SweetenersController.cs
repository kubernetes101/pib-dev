// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using CoffeeShop.Model;
using CseLabs.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CoffeeShop.Controllers
{
    /// <summary>
    /// Handle Sweeteners requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SweetenersController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(SweetenersController).FullName,
            ErrorMessage = "SweetenersControllerException",
        };

        /// <summary>
        /// Get all Sweeteners
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetSweeteners")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Sweetener>))]
        [SwaggerOperation(Summary = "Get all Sweeteners", Description = "Get all Sweeteners")]
        public IActionResult GetSweeteners()
        {
            Logger.LogInformation("get", "GetSweeteners");

            Database db = new();

            return Ok(db.Sweeteners.Values);
        }
    }
}
