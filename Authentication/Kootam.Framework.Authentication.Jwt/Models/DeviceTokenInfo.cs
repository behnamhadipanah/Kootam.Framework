using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Authentication.Jwt.Models;

public class DeviceTokenInfo
{
    public string DeviceType { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessAt { get; set; }
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
}
