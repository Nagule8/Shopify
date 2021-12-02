using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopApi.Entity
{
    public enum Role
    {
        Administrator = 0,
        SuperSu = 1,
        User = 2
    }
}
