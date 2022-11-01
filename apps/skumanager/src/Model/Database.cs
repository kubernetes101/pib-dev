// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using SkuManager.Model;

namespace SkuManager
{
    /// <summary>
    /// Database class (singleton)
    /// </summary>
    public sealed class Database
    {
        private readonly Dictionary<string, SKU> skus;

        public Database()
        {
            if (File.Exists(App.Config.DataFilePath))
            {
                skus = JsonSerializer.Deserialize<Dictionary<string, SKU>>(File.ReadAllText(App.Config.DataFilePath), JsonOptions);
            }
            else
            {
                if (File.Exists(App.Config.SwaggerFilePath))
                {
                    var sw = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(App.Config.SwaggerFilePath), JsonOptions);

                    if (sw.ContainsKey("x-sample-data"))
                    {
                        string json = JsonSerializer.Serialize<object>(sw["x-sample-data"]);
                        skus = JsonSerializer.Deserialize<Dictionary<string, SKU>>(json);
                    }
                }
            }
        }

        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            //DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,

            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        public Dictionary<string, SKU> SKUs
        {
            get
            {
                if (skus == null)
                {
                    return new();
                }

                return skus;
            }
        }

        public void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(App.Config.DataFilePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(App.Config.DataFilePath, JsonSerializer.Serialize(skus, JsonOptions));
            }
            catch (Exception ex)
            {
                // todo - structured error
                Console.WriteLine($"Database.Save.Exception: {ex.Message}");
            }
        }

        public bool IsValid(SKU sku, bool shouldExist)
        {
            if (sku == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(sku.Id))
            {
                return false;
            }

            if (shouldExist != skus.ContainsKey(sku.Id))
            {
                return false;
            }

            if (string.IsNullOrEmpty(sku.Name))
            {
                return false;
            }

            if (string.IsNullOrEmpty(sku.Sku))
            {
                return false;
            }

            if (string.IsNullOrEmpty(sku.Description))
            {
                return false;
            }

            if (sku.Price <= 0)
            {
                return false;
            }

            return true;
        }

        public void UpdateSKU(SKU sku)
        {
            if (skus != null)
            {
                skus[sku.Id] = sku;
            }
        }

        public void DeleteSKU(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (skus.ContainsKey(id))
            {
                skus.Remove(id);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Database);
        }

        public bool Equals(Database other)
        {
            return true;
        }
    }
}
