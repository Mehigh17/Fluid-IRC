using System;
using System.ComponentModel.DataAnnotations;

namespace FluidIrc.Model.Models
{
    public class Server
    {

        [Key] public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public bool SslEnabled { get; set; }

        public UserProfile UserProfile { get; set; }

    }
}
