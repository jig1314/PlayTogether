using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class AppSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EnumCode { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
