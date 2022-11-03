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

        /// <summary>
        /// Get all Coffees
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetCoffee")]
        [Route("api/v1/[controller]/coffee")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Coffees", Description = "Get all Coffees")]
        public IActionResult GetCoffees()
        {
            Logger.LogInformation("get", "GetCoffee");
            return Ok(GetDrinksByCategory("Coffee"));
        }

        /// <summary>
        /// Get all Teas
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetTea")]
        [Route("api/v1/[controller]/tea")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Teas", Description = "Get all Teas")]
        public IActionResult GetTea()
        {
            Logger.LogInformation("get", "GetTea");
            return Ok(GetDrinksByCategory("Tea"));
        }

        /// <summary>
        /// Get all Lemonade
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetCoffee")]
        [Route("api/v1/[controller]/lemonade")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Lemonade", Description = "Get all Lemonade")]
        public IActionResult GetLemonade()
        {
            Logger.LogInformation("get", "GetLemonade");
            return Ok(GetDrinksByCategory("Lemonade"));
        }

        /// <summary>
        /// Get all Water
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetWater")]
        [Route("api/v1/[controller]/water")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Water", Description = "Get all Water")]
        public IActionResult GetWater()
        {
            Logger.LogInformation("get", "GetWater");
            return Ok(GetDrinksByCategory("Water"));
        }

        private static List<Drink> GetDrinksByCategory(string category)
        {
            Database db = new();

            List<Drink> list = new();

            foreach (var d in db.Drinks.Values)
            {
                if (d.Category == category)
                {
                    list.Add(d);
                }
            }

            return list;
        }
    }
}
