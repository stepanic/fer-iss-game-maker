using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSProject.Models
{
    public class GoNoGoParameters
    {
        public int SectionNum { get; set; }
        public IList<int> SectionSamplings { get; set; }
        public IList<TimeSpan> SectionDurations { get; set; } 
        
        public GoNoGoParameters()
        {
            SectionSamplings = new List<int>();
            SectionDurations = new List<TimeSpan>();
        }
    }
}
