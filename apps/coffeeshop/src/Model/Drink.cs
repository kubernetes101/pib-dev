// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace CoffeeShop.Model
{
    public enum IceEnum
    {
        NA,
        NormalIce,
        NoIce,
        LightIce,
        ExtraIce,
    }

    public enum FoamEnum
    {
        NA,
        NormalFoam,
        NoFoam,
        LightFoam,
        HeavyFoam,
    }

    /// <summary>
    /// Drink model
    /// </summary>
    public partial class Drink
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsHot { get; set; }
        public IceEnum Ice { get; set; }
        public FoamEnum Foam { get; set; }
        public Dictionary<string, Size> Sizes { get; set; } = new();
        public Dictionary<string, Sweetener> Sweeteners { get; set; } = new();
        public Dictionary<string, Milk> Milks { get; set; } = new();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Drink);
        }

        public bool Equals(Drink other)
        {
            if (other == null ||
            Id != other.Id ||
            Name != other.Name ||
            IsHot != other.IsHot)
            {
                return false;
            }

            return true;
        }

        public Drink DeepCopy()
        {
            Drink tgt = new()
            {
                Id = this.Id,
                Name = this.Name,
                IsHot = this.IsHot,
            };

            if (Sizes != null)
            {
                tgt.Sizes = new();
            }

            if (Sweeteners != null)
            {
                tgt.Sweeteners = new();
            }

            if (Milks != null)
            {
                tgt.Milks = new();
            }

            return tgt;
        }
    }
}
