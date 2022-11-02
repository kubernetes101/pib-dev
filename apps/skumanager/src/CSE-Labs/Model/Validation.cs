// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace InventoryDataService.Model
{
    /// <summary>
    /// Validation methods
    /// </summary>
    public class Validation
    {
        public static bool Validate(Dictionary<string, object> source, Dictionary<string, object> other)
        {
            if (source == null)
            {
                if (other != null)
                {
                    return false;
                }
            }
            else if (other == null)
            {
                return false;
            }
            else if (source.Count != other.Count)
            {
                return false;
            }
            else
            {
                foreach (KeyValuePair<string, object> d in source)
                {
                    // todo - this validates strings
                    if (!other.ContainsKey(d.Key) || other[d.Key].ToString() != d.Value.ToString())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool Validate(List<string> source, List<string> other)
        {
            if (source == null)
            {
                if (other != null)
                {
                    return false;
                }
            }
            else if (other == null)
            {
                return false;
            }
            else if (source.Count != other.Count)
            {
                return false;
            }
            else
            {
                foreach (string cl in source)
                {
                    if (!other.Contains(cl))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
