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
    [Route("api/v1/drinks/[controller]")]
    [Produces("application/json")]
    public class CoffeeController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(CoffeeController).FullName,
            ErrorMessage = $"{typeof(CoffeeController).FullName}Exception",
        };

        /// <summary>
        /// Get all Coffees
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetCoffee")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Coffees", Description = "Get all Coffees")]
        public IActionResult GetCoffees()
        {
            Logger.LogInformation("get", "GetCoffee");

            Database db = new();

            List<Drink> list = new();

            foreach (var d in db.Drinks.Values)
            {
                if (d.Category == "Coffee")
                {
                    list.Add(d);
                }
            }

            return Ok(list);
        }
    }
}
