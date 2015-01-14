using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSProject.Models
{
    public class Stimulus
    {
        public string Guid { get; set; }
        public int Priority { get; set; }
        public string Label { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTme { get; set; }
        public StimulusType Type { get; set; }

        public Stimulus()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    }

    public enum StimulusType
    {
        Image = 0, Video = 1, Sound = 2, Text = 3, Game = 4
    }
}
