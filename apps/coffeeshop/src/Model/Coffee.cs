// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace CoffeeShop.Model
{
    /// <summary>
    /// Coffee model
    /// </summary>
    public partial class Coffee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsHot { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Coffee);
        }

        public bool Equals(Coffee other)
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

        public Coffee DeepCopy()
        {
            Coffee tgt = new()
            {
                Id = this.Id,
                Name = this.Name,
                IsHot = this.IsHot,
            };

            return tgt;
        }
    }
}
