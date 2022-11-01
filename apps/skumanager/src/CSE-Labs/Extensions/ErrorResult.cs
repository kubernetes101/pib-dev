// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net;

namespace CseLabs.Middleware
{
    public class ErrorResult
    {
        public int Status => (int)Error;

        public string Message { get; set; }

        public HttpStatusCode Error { get; set; }
    }
}
