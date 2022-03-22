using System;
using System.Collections.Generic;

namespace sheift.Models
{
    public partial class Shieft
    {
        public int ShiftId { get; set; }
        public string Date { get; set; } = null!;
        public int UserId { get; set; }
        public int AdminId { get; set; }
        public string Time { get; set; } = null!;
        public int ShiftTypeId { get; set; }
    }
}
