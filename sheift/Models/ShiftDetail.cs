using System;
using System.Collections.Generic;

namespace sheift.Models
{
    public partial class ShiftDetail
    {
        public int ShiftId { get; set; }
        public string Date { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int AdminId { get; set; }
        public string AdminName { get; set; } = null!;
        public string ShiftName { get; set; } = null!;
        public string DepName { get; set; } = null!;
    }
}
