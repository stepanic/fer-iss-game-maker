using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSProject.Models
{
    public class GameContext
    {
        public TimeSpan Duration { get; set; }
        public IList<Stimulus> StimulusList { get; set; }

        public GameContext()
        {
            StimulusList = new List<Stimulus>();
        }
    }
}
