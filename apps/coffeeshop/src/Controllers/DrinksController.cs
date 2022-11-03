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
        /// Get all Drinks
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetDrinks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Drinks", Description = "Get all Drinks")]
        public IActionResult GetDrinks()
        {
            Logger.LogInformation("get", "GetDrinks");

            Database db = new();

            return Ok(db.Drinks.Values);
        }
    }
}
