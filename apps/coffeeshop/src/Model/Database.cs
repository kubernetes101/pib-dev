// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoffeeShop.Model
{
    /// <summary>
    /// Database class (singleton)
    /// </summary>
    public sealed class Database
    {
        private readonly Inventory inventory;

        public Database()
        {
            if (File.Exists(App.Config.DataFilePath))
            {
                inventory = JsonSerializer.Deserialize<Inventory>(File.ReadAllText(App.Config.DataFilePath));
            }
            else
            {
                if (File.Exists(App.Config.SwaggerFilePath))
                {
                    var sw = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(App.Config.SwaggerFilePath), JsonOptions);

                    if (sw.ContainsKey("x-sampleData"))
                    {
                        var json = JsonSerializer.Serialize<object>(sw["x-sampleData"], JsonOptions);

                        inventory = JsonSerializer.Deserialize<Inventory>(json, JsonOptions);
                    }
                }
            }

            inventory.Drinks ??= new();
            inventory.Food ??= new();
            inventory.Sweeteners ??= new();
            inventory.Milks ??= new();
            inventory.Sizes ??= new();
        }

        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,

            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            AllowTrailingCommas = true,

            Converters = { new JsonStringEnumConverter() },
        };

        public Dictionary<string, Drink> Drinks
        {
            get
            {
                if (inventory == null || inventory.Drinks == null)
                {
                    return new();
                }

                return inventory.Drinks;
            }
        }

        public Dictionary<string, Milk> Milks
        {
            get
            {
                if (inventory == null || inventory.Milks == null)
                {
                    return new();
                }

                return inventory.Milks;
            }
        }

        public Dictionary<string, Size> Sizes
        {
            get
            {
                if (inventory == null || inventory.Sizes == null)
                {
                    return new();
                }

                return inventory.Sizes;
            }
        }

        public Dictionary<string, Sweetener> Sweeteners
        {
            get
            {
                if (inventory == null || inventory.Sweeteners == null)
                {
                    return new();
                }

                return inventory.Sweeteners;
            }
        }

        public Dictionary<string, FoodItem> Foods
        {
            get
            {
                if (inventory == null || inventory.Food == null)
                {
                    return new();
                }

                return inventory.Food;
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

                File.WriteAllText(App.Config.DataFilePath, JsonSerializer.Serialize(inventory, JsonOptions));
            }
            catch (Exception ex)
            {
                // todo - structured error
                Console.WriteLine($"Database.Save.Exception: {ex.Message}");
            }
        }

        public bool IsValid(Drink drink, bool shouldExist)
        {
            if (drink == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(drink.Id))
            {
                return false;
            }

            if (shouldExist != Drinks.ContainsKey(drink.Id))
            {
                return false;
            }

            return true;
        }

        public bool IsValid(Size size, bool shouldExist)
        {
            if (size == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(size.Id))
            {
                return false;
            }

            if (shouldExist != Sweeteners.ContainsKey(size.Id))
            {
                return false;
            }

            return true;
        }

        public bool IsValid(Sweetener sw, bool shouldExist)
        {
            if (sw == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(sw.Id))
            {
                return false;
            }

            if (shouldExist != Sizes.ContainsKey(sw.Id))
            {
                return false;
            }

            return true;
        }

        public void UpdateDrinks(Drink drink)
        {
            if (drink != null && inventory != null && inventory.Drinks != null)
            {
                inventory.Drinks[drink.Id] = drink;
            }
        }

        public void UpdateSize(Size sz)
        {
            if (sz == null)
            {
                return;
            }

            inventory.Sizes[sz.Id] = sz;
        }

        public void UpdateSweetener(Sweetener sweet)
        {
            if (sweet == null)
            {
                return;
            }

            inventory.Sweeteners[sweet.Id] = sweet;
        }

        public void DeleteDrinks(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (inventory.Drinks.ContainsKey(id))
            {
                inventory.Drinks.Remove(id);
            }
        }

        public void DeleteSize(string id)
        {
            if (inventory.Sizes.ContainsKey(id))
            {
                inventory.Sizes.Remove(id);
            }
        }

        public void DeleteSweetener(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (inventory.Sweeteners.ContainsKey(id))
            {
                inventory.Sweeteners.Remove(id);
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
            if (other == null)
            {
                return false;
            }

            if (other.inventory == null ||
                other.inventory.Drinks == null ||
                other.inventory.Food == null ||
                other.inventory.Milks == null ||
                other.inventory.Sizes == null ||
                other.inventory.Sweeteners == null)
            {
                return false;
            }

            if (other.inventory.Drinks.Count != inventory.Drinks.Count ||
                other.inventory.Food.Count != inventory.Food.Count ||
                other.inventory.Milks.Count != inventory.Milks.Count ||
                other.inventory.Sizes.Count != inventory.Sizes.Count ||
                other.inventory.Sweeteners.Count != inventory.Sweeteners.Count)
            {
                return false;
            }

            return true;
        }
    }
}
