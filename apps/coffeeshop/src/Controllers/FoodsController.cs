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
    public class FoodsController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(FoodsController).FullName,
            ErrorMessage = $"{typeof(FoodsController).FullName}Exception",
        };

        /// <summary>
        /// Get all Food Items
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetFood")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FoodItem>))]
        [SwaggerOperation(Summary = "Get all Food Items", Description = "Get all Food Items")]
        public IActionResult GetFood()
        {
            Logger.LogInformation("get", "GetFood");

            Database db = new();

            return Ok(db.Foods.Values);
        }
    }
}
