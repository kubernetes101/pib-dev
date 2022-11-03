// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace CoffeeShop.Model
{
    /// <summary>
    /// Milk model
    /// </summary>
    public partial class Milk
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Milk);
        }

        public bool Equals(Milk other)
        {
            if (other == null ||
              Id != other.Id ||
              Name != other.Name)
            {
                return false;
            }

            return true;
        }

        public Milk DeepCopy()
        {
            Milk tgt = new()
            {
                Id = this.Id,
                Name = this.Name,
            };

            return tgt;
        }
    }
}
