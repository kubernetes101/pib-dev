// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using CseLabs.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SkuManager.Model;

namespace SkuManager.Controllers
{
    /// <summary>
    /// Handle Skus requests
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SkusController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(SkusController).FullName,
            ErrorMessage = "SkusControllerException",
        };

        /// <summary>
        /// Get all SKUs
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet(Name = "GetSKUs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Sku>))]
        [SwaggerOperation(Summary = "Get all SKUs", Description = "Get all SKUs")]
        public IActionResult GetSKUs()
        {
            Logger.LogInformation("get", "GetSKUs");

            Database db = new();

            return Ok(db.Skus.Values);
        }

        /// <summary>
        /// Add SKU
        /// </summary>
        /// <param name="sku">Sku in json format</param>
        /// <returns>IActionResult</returns>
        [HttpPost(Name = "AddSKU")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Add SKU", Description = "Add SKU")]
        public IActionResult AddSKU([FromBody] Sku sku)
        {
            Logger.LogInformation("post", "AddSKU");

            Database db = new();

            if (!db.IsValid(sku, false))
            {
                return BadRequest("Invalid Sku Data");
            }

            db.UpdateSku(sku);
            db.Save();

            return Created($"/api/v1/skus/{sku.SkuId}", null);
        }

        /// <summary>
        /// Get SKU by Id
        /// </summary>
        /// <param name="id">Sku ID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}", Name = "GetSKUById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Sku))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get SKU by Id", Description = "Get SKU by Id")]
        public IActionResult GetSKUById([FromRoute] string id)
        {
            Logger.LogInformation("get", "GetSKUById");

            Database db = new();

            if (string.IsNullOrEmpty(id) || !db.Skus.ContainsKey(id))
            {
                return NotFound("Sku not found");
            }

            return Ok(db.Skus[id]);
        }

        /// <summary>
        /// Delete SKU
        /// </summary>
        /// <param name="id">Sku ID</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}", Name = "DeleteSKU")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Summary = "Delete SKU", Description = "Delete SKU")]
        public IActionResult DeleteSKU([FromRoute] string id)
        {
            Logger.LogInformation("delete", $"{id}");

            Database db = new();
            db.DeleteSku(id);
            db.Save();

            return NoContent();
        }

        /// <summary>
        /// Update SKU
        /// </summary>
        /// <param name="id">Sku ID</param>
        /// <param name="sku">Sku in json body</param>
        /// <returns>IActionResult</returns>
        [HttpPut("{id}", Name = "UpdateApp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update SKU", Description = "Update SKU")]
        public IActionResult UpdateApp([FromRoute] string id, [FromBody] Sku sku)
        {
            Logger.LogInformation("put", "UpdateApp");

            Database db = new();

            if (!db.IsValid(sku, false) && !db.IsValid(sku, true))
            {
                return BadRequest("Invalid Sku Data");
            }

            if (sku.SkuId != id)
            {
                return BadRequest("id param doesn't match json SkuId");
            }

            db.UpdateSku(sku);
            db.Save();

            if (db.IsValid(sku, false))
            {
                return Created($"/api/v1/skus/{sku.SkuId}", null);
            }

            return NoContent();
        }

        /// <summary>
        /// Get SKUs by Category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>IActionResult</returns>
        [HttpGet("category/{categoryId}", Name = "GetSKUsByCategoryId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Sku))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get SKUs by Category", Description = "Get SKUs by Category")]
        public IActionResult GetSKUsByCategoryId([FromRoute] string categoryId)
        {
            Logger.LogInformation("get", "GetSKUsByCategoryId");

            Database db = new();

            if (string.IsNullOrEmpty(categoryId))
            {
                return NotFound("Sku not found");
            }

            List<Sku> list = new();

            foreach (Sku s in db.Skus.Values)
            {
                if (s.CategoryId.Equals(categoryId, System.StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(s);
                }
            }

            return Ok(list);
        }
    }
}
