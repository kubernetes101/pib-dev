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
    /// Handle Modifiers requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ModifiersController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(ModifiersController).FullName,
            ErrorMessage = "ModifiersControllerException",
        };

        /// <summary>
        /// Get all Sizes
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetSizes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Size>))]
        [SwaggerOperation(Summary = "Get all Sizes", Description = "Get all Sizes")]
        public IActionResult GetSizes()
        {
            Logger.LogInformation("get", "GetSizes");

            Database db = new();

            return Ok(db.Sizes.Values);
        }
    }
}
