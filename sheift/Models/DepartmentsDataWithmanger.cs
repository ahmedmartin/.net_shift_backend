using System;
using System.Collections.Generic;

namespace sheift.Models
{
    public partial class DepartmentsDataWithmanger
    {
        public int DepId { get; set; }
        public string DepName { get; set; } = null!;
        public int MangerId { get; set; }
    }
}
