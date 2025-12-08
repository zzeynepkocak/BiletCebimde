
﻿using Microsoft.AspNetCore.Identity;

namespace BiletCebimde.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
