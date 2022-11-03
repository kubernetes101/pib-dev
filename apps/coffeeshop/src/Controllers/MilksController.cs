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
    /// Handle Milks requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class MilksController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(MilksController).FullName,
            ErrorMessage = "MilksControllerException",
        };

        /// <summary>
        /// Get all Milks
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetMilks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Milk>))]
        [SwaggerOperation(Summary = "Get all Milks", Description = "Get all Milks")]
        public IActionResult GetMilks()
        {
            Logger.LogInformation("get", "GetMilks");

            Database db = new();

            return Ok(db.Milks.Values);
        }
    }
}
