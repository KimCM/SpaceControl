using System;

namespace Hacksaar.SpaceControl.WP80
{
    public class SpaceControlLogEntry
    {
        public DateTime CreatedAt { get; set; }

        public SpaceControlLogSeverity Severity { get; set; }

        public string Text { get; set; }
    }
}