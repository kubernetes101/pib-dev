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
    /// Handle Coffees requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CoffeesController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(CoffeesController).FullName,
            ErrorMessage = "CoffeesControllerException",
        };

        /// <summary>
        /// Get all Coffees
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetCoffees")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Coffee>))]
        [SwaggerOperation(Summary = "Get all Coffees", Description = "Get all Coffees")]
        public IActionResult GetCoffees()
        {
            Logger.LogInformation("get", "GetCoffees");

            Database db = new();

            return Ok(db.Coffees.Values);
        }
    }
}
