// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SkuManager.Model
{
  /// <summary>
  /// SKU model
  /// </summary>
  public partial class SKU
  {
    [JsonPropertyName("sku")]
    public string Sku { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; }
    [JsonPropertyName("price")]
    public double Price { get; set; }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SKU);
    }

    public bool Equals(SKU other)
    {
      if (other == null ||
        Sku != other.Sku ||
        Id != other.Id ||
        Name != other.Name ||
        Description != other.Description ||
        CategoryId != other.CategoryId ||
        Price != other.Price)
      {
        return false;
      }


      return true;
    }

    public SKU DeepCopy()
    {
      SKU tgt = new()
      {
        Sku = this.Sku,
        Id = this.Id,
        Name = this.Name,
        Description = this.Description,
        CategoryId = this.CategoryId,
        Price = this.Price,
      };


      return tgt;
    }
  }
}
