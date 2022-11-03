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
    /// Handle Breakfast requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class BreakfastController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(BreakfastController).FullName,
            ErrorMessage = "BreakfastControllerException",
        };

        /// <summary>
        /// Get all Breakfast Items
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetBreakfast")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FoodItem>))]
        [SwaggerOperation(Summary = "Get all Breakfast Items", Description = "Get all Breakfast Items")]
        public IActionResult GetBreakfast()
        {
            Logger.LogInformation("get", "GetBreakfast");

            Database db = new();

            return Ok(db.Breakfast.Values);
        }
    }
}
