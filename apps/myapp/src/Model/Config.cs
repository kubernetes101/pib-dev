// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace MyApp
{
    public enum AppType
    {
        /// <summary>
        /// The app type.
        /// </summary>
        App,

        /// <summary>
        /// The web API type.
        /// </summary>
        WebAPI,
    }

    public class Config
    {
        public string SecretsVolume { get; set; } = "secrets";
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public bool IsLogLevelSet { get; set; }
        public Secrets Secrets { get; set; }
        public bool DryRun { get; set; }
        public int CacheDuration { get; set; } = 300;
        public string Zone { get; set; }
        public string Region { get; set; }
        public int Port { get; set; } = 8080;
        public int Retries { get; set; } = 10;
        public int Timeout { get; set; } = 10;
        public LogLevel RequestLogLevel { get; set; } = LogLevel.Information;

        public void SetConfig(Config config)
        {
            IsLogLevelSet = config.IsLogLevelSet;
            DryRun = config.DryRun;
            CacheDuration = config.CacheDuration;
            Secrets = config.Secrets;
            Port = config.Port;
            Retries = config.Retries;
            Timeout = config.Timeout;

            // LogLevel.Information is the min
            LogLevel = config.LogLevel <= LogLevel.Information ? LogLevel.Information : config.LogLevel;
            RequestLogLevel = config.RequestLogLevel <= LogLevel.Information ? LogLevel.Information : config.RequestLogLevel;

            // clean up string values
            SecretsVolume = string.IsNullOrWhiteSpace(config.SecretsVolume) ? string.Empty : config.SecretsVolume.Trim();
            Zone = string.IsNullOrWhiteSpace(config.Zone) ? string.Empty : config.Zone.Trim();
            Region = string.IsNullOrWhiteSpace(config.Region) ? string.Empty : config.Region.Trim();
        }
    }
}
