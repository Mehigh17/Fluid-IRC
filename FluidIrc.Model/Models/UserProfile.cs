using System;
using System.ComponentModel.DataAnnotations;

namespace FluidIrc.Model.Models
{
    public class UserProfile
    {

        [Key]
        public Guid Id { get; set; }

        public string Nickname { get; set; }

    }
}
