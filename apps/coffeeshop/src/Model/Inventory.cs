// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace CoffeeShop.Model
{
  /// <summary>
  /// Coffee model
  /// </summary>
  public partial class Inventory
  {
        public Dictionary<string, Coffee> Coffees { get; set; }
        public Dictionary<string, Tea> Teas { get; set; }
        public Dictionary<string, Sweetener> Sweeteners { get; set; }
        public Dictionary<string, Milk> Milks { get; set; }
        public Dictionary<string, Size> Sizes { get; set; }
    }
}
