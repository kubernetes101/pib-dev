// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using CseLabs.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using CoffeeShop.Model;

namespace CoffeeShop.Controllers
{
    /// <summary>
    /// Handle Drinks requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class DrinksController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(DrinksController).FullName,
            ErrorMessage = "DrinksControllerException",
        };

        /// <summary>
        /// Get all Teas
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetTeas")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Tea>))]
        [SwaggerOperation(Summary = "Get all Teas", Description = "Get all Teas")]
        public IActionResult GetTeas()
        {
            Logger.LogInformation("get", "GetTeas");

            Database db = new();

            return Ok(db.Teas.Values);
        }
    }
}
