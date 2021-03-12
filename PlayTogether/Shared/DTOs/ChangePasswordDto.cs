using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
