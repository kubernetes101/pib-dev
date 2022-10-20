// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    /// <summary>
    /// Handle the /readyz requests
    ///
    /// </summary>
    [Route("[controller]")]
    public class ReadyzController : Controller
    {
        /// <summary>
        /// Returns a plain text ready status
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult RunReadyzAsync()
        {
            return Ok("ready");
        }
    }
}
