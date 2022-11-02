// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SkuManager.Model
{
  /// <summary>
  /// Sku model
  /// </summary>
  public partial class Sku
  {
    public string Value { get; set; }
    public string SkuId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public double Price { get; set; }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Sku);
    }

    public bool Equals(Sku other)
    {
      if (other == null ||
        Value != other.Value ||
        SkuId != other.SkuId ||
        Name != other.Name ||
        Description != other.Description ||
        CategoryId != other.CategoryId ||
        Price != other.Price)
      {
        return false;
      }

      return true;
    }

    public Sku DeepCopy()
    {
      Sku tgt = new()
      {
        Value = this.Value,
        SkuId = this.SkuId,
        Name = this.Name,
        Description = this.Description,
        CategoryId = this.CategoryId,
        Price = this.Price,
      };

      return tgt;
    }
  }
}
