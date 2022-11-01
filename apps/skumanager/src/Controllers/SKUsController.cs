// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using CseLabs.Middleware;
using SkuManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Swashbuckle.AspNetCore.Annotations;

namespace SkuManager.Controllers
{
    /// <summary>
    /// Handle SKUs requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SKUsController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(SKUsController).FullName,
            ErrorMessage = "SKUsControllerException",
        };

        /// <summary>
        /// Returns an array of SKU
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetSKUs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SKU>))]
        [SwaggerOperation(Summary = "Get all SKUs", Description = "Get all SKUs")]
        public IActionResult GetSKUs()
        {
            Logger.LogInformation("get", "GetSKUs");

            Database db = new();

            return Ok(db.SKUs.Values);
        }

        /// <summary>
        /// Returns SKU by ID
        /// </summary>
        /// <param name="id">SKU ID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}", Name = "GetSKUById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SKU))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get SKU by Id", Description = "Get SKU by Id")]
        public IActionResult GetSKUById([FromRoute] string id)
        {
            Logger.LogInformation("get", "GetSKUById");

            Database db = new();

            if (string.IsNullOrEmpty(id) || !db.SKUs.ContainsKey(id))
            {
                return NotFound("SKU not found");
            }

            return Ok(db.SKUs[id]);
        }

        /// <summary>
        /// Add SKU
        /// </summary>
        /// <param name="sku">SKU in json format</param>
        /// <returns>IActionResult</returns>
        [HttpPost(Name = "AddSKU")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Add SKU", Description = "Add SKU")]
        public IActionResult AddSKU([FromBody] SKU sku)
        {
            Logger.LogInformation("post", "AddSKU");

            Database db = new();

            if (!db.IsValid(sku, false))
            {
                return BadRequest("Invalid SKU Data");
            }

            db.UpdateSKU(sku);
            db.Save();

            return Created($"/api/v1/skus/{sku.Id}", null);
        }

        /// <summary>
        /// Update SKU by ID
        /// </summary>
        /// <param name="id">SKU ID</param>
        /// <param name="sku">SKU in json body</param>
        /// <returns>IActionResult</returns>
        [HttpPut("{id}", Name = "UpdateApp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update SKU", Description = "update SKU")]
        public IActionResult UpdateApp([FromRoute] string id, [FromBody] SKU sku)
        {
            Logger.LogInformation("put", "UpdateApp");

            Database db = new();

            if (!db.IsValid(sku, false) && !db.IsValid(sku, true))
            {
                return BadRequest("Invalid SKU Data");
            }

            if (sku.Id != id)
            {
                return BadRequest("id param doesn't match json SKUId");
            }

            db.UpdateSKU(sku);
            db.Save();

            if (db.IsValid(sku, false))
            {
                return Created($"/api/v1/skus/{sku.Id}", null);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete SKU by ID
        /// </summary>
        /// <param name="id">SKU ID</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}", Name = "DeleteSKU")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Summary = "Delete SKU", Description = "Delete SKU")]
        public IActionResult DeleteSKU([FromRoute] string id)
        {
            Logger.LogInformation("delete", $"{id}");

            Database db = new();
            db.DeleteSKU(id);
            db.Save();

            return NoContent();
        }
    }
}
