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

            inventory.Coffees ??= new();
            inventory.Teas ??= new();
            inventory.Sweeteners ??= new();
            inventory.Milks ??= new();
            inventory.Sizes ??= new();
            inventory.Breakfast ??= new();
            inventory.Lunch ??= new();
            inventory.Snacks ??= new();
            inventory.Drinks ??= new();
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

            Converters =
            {
                new JsonStringEnumConverter()
            },
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

        public Dictionary<string, Coffee> Coffees
        {
            get
            {
                if (inventory == null || inventory.Coffees == null)
                {
                    return new();
                }

                return inventory.Coffees;
            }
        }

        public Dictionary<string, Tea> Teas
        {
            get
            {
                if (inventory == null || inventory.Teas == null)
                {
                    return new();
                }

                return inventory.Teas;
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

        public Dictionary<string, FoodItem> Breakfast
        {
            get
            {
                if (inventory == null || inventory.Breakfast == null)
                {
                    return new();
                }

                return inventory.Breakfast;
            }
        }

        public Dictionary<string, FoodItem> Lunch
        {
            get
            {
                if (inventory == null || inventory.Lunch == null)
                {
                    return new();
                }

                return inventory.Lunch;
            }
        }

        public Dictionary<string, FoodItem> Snacks
        {
            get
            {
                if (inventory == null || inventory.Snacks == null)
                {
                    return new();
                }

                return inventory.Snacks;
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

        public bool IsValid(Coffee c, bool shouldExist)
        {
            if (c == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(c.Id))
            {
                return false;
            }

            if (shouldExist != Coffees.ContainsKey(c.Id))
            {
                return false;
            }

            if (string.IsNullOrEmpty(c.Name))
            {
                return false;
            }

            return true;
        }

        public bool IsValid(Tea tea, bool shouldExist)
        {
            if (tea == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(tea.Id))
            {
                return false;
            }

            if (shouldExist != Teas.ContainsKey(tea.Id))
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

        public void UpdateCoffee(Coffee cf)
        {
            if (cf != null && inventory != null && inventory.Coffees != null)
            {
                inventory.Coffees[cf.Id] = cf;
            }
        }

        public void UpdateTeas(Tea tea)
        {
            if (tea == null)
            {
                return;
            }

            inventory.Teas[tea.Id] = tea;
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

        public void DeleteCoffee(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (inventory.Coffees.ContainsKey(id))
            {
                inventory.Coffees.Remove(id);
            }
        }

        public void DeleteTeas(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            if (inventory.Teas.ContainsKey(id))
            {
                inventory.Teas.Remove(id);
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
                other.inventory.Coffees == null ||
                other.inventory.Teas == null ||
                other.inventory.Milks == null ||
                other.inventory.Sizes == null ||
                other.inventory.Sweeteners == null)
            {
                return false;
            }

            if (other.inventory.Coffees.Count != inventory.Coffees.Count ||
                other.inventory.Teas.Count != inventory.Teas.Count ||
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
