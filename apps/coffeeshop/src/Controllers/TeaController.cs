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
    /// Handle Teas requests
    /// </summary>
    [Route("api/v1/drinks/[controller]")]
    [Produces("application/json")]
    public class TeaController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(TeaController).FullName,
            ErrorMessage = $"{typeof(TeaController).FullName}Exception",
        };

        /// <summary>
        /// Get all Drinks that are Tea
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetTeas")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Drink>))]
        [SwaggerOperation(Summary = "Get all Drinks that are Tea", Description = "Get all Drinks that are Tea")]
        public IActionResult GetTea()
        {
            Logger.LogInformation("get", "GetTea");

            Database db = new();

            List<Drink> list = new();

            foreach (var d in db.Drinks.Values)
            {
                if (d.Category == "Tea")
                {
                    list.Add(d);
                }
            }

            return Ok(list);
        }
    }
}
