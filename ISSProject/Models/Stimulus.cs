using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSProject.Models
{
    public class Stimulus
    {
        public int Priority { get; set; }
        public string Label { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTme { get; set; }
    }

    public enum StimulusType
    {
        Image = 0, Video = 1, Sound = 2, Text = 3
    }
}
