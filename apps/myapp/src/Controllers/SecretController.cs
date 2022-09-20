// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Net;
using CseLabs.Middleware;
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
        /// <response code="200">text/plain with secret value</response code>
        /// <returns>IActionResult</returns>
        [HttpGet("{key}")]
        public IActionResult GetSecret([FromRoute] string key)
        {
            try
            {
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

                return new ObjectResult("secret not found")
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
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
