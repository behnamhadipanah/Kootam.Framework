using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Authentication.Jwt.Models;

public record ResponseLogin(string Token,RefreshToken RefreshToken);
