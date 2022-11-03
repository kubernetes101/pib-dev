// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace CoffeeShop.Model
{
  /// <summary>
  /// FoodItem model
  /// </summary>
  public partial class FoodItem
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FoodItem);
    }

    public bool Equals(FoodItem other)
    {
      if (other == null ||
        Id != other.Id ||
        Name != other.Name ||
        Price != other.Price)
      {
        return false;
      }


      return true;
    }

    public FoodItem DeepCopy()
    {
      FoodItem tgt = new()
      {
        Id = this.Id,
        Name = this.Name,
        Price = this.Price,
      };


      return tgt;
    }
  }
}
