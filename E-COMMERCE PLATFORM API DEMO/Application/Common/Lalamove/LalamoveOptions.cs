using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public class LalamoveOptions
    {
        public string BaseUrl { get; set; } = null!;
        public string Market { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
    }
}
