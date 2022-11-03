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
    /// Handle Lunch requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class LunchController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(LunchController).FullName,
            ErrorMessage = "LunchControllerException",
        };

        /// <summary>
        /// Get all Lunch Items
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetLunch")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FoodItem>))]
        [SwaggerOperation(Summary = "Get all Lunch Items", Description = "Get all Lunch Items")]
        public IActionResult GetLunch()
        {
            Logger.LogInformation("get", "GetLunch");

            Database db = new();

            return Ok(db.Lunch.Values);
        }
    }
}
