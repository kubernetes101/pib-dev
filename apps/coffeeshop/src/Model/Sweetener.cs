// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace CoffeeShop.Model
{
    /// <summary>
    /// Sweetener model
    /// </summary>
    public partial class Sweetener
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Sweetener);
        }

        public bool Equals(Sweetener other)
        {
            if (other == null ||
              Id != other.Id ||
              Name != other.Name)
            {
                return false;
            }

            return true;
        }

        public Sweetener DeepCopy()
        {
            Sweetener tgt = new()
            {
                Id = this.Id,
                Name = this.Name,
            };

            return tgt;
        }
    }
}
