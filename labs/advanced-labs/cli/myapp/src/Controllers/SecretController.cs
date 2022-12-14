// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Net;
using CseLabs.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    /// <summary>
    /// Handle secret requests
    /// </summary>
    [Route("api/[controller]")]
    public class SecretController : Controller
    {
        private static readonly CseLog Logger = new ()
        {
            Name = typeof(SecretController).FullName,
            ErrorMessage = "SecretControllerException",
        };

        /// <summary>
        /// Returns the Key Vault secret mounted in the secrets volume
        /// </summary>
        /// <param name="key">key for secret</param>
        /// <response code="200"></response>
        /// <returns>IActionResult</returns>
        [HttpGet("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSecret([FromRoute] string key)
        {
            try
            {
                // return the value of secret if found
                if (!string.IsNullOrWhiteSpace(key))
                {
                    string secret = string.Empty;
                    if (App.Config.Secrets.GetSecretsFromVolume().TryGetValue(key.Trim(), out secret))
                    {
                        if (!string.IsNullOrWhiteSpace(secret))
                        {
                            return Ok(secret);
                        }
                    }
                }

                return NotFound("secret not found");
            }
            catch (Exception ex)
            {
                Logger.LogError(nameof(GetSecret), $"GetSecret Exception: {ex.Message}");

                return new ObjectResult("getsecret exception")
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                };
            }
        }
    }
}
