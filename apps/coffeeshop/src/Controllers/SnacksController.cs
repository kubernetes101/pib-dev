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
    /// Handle Snacks requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SnacksController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(SnacksController).FullName,
            ErrorMessage = "SnacksControllerException",
        };

        /// <summary>
        /// Get all Snacks Items
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetSnacks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FoodItem>))]
        [SwaggerOperation(Summary = "Get all Snacks Items", Description = "Get all Snacks Items")]
        public IActionResult GetSnacks()
        {
            Logger.LogInformation("get", "GetSnacks");

            Database db = new();

            return Ok(db.Snacks.Values);
        }
    }
}
