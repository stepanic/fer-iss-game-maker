using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSProject.Models
{
    public class GoNoGoResult
    {
        public int Misses { get; set; }
        public int Hits { get; set; }
        public int Errors { get; set; }
        /// <summary>
        /// Reaction times in ms
        /// </summary>
        public IList<int> ReactionTimes { get; set; }

        public double AverageReactionTime { get { return ReactionTimes.Average(); } }


        public GoNoGoResult()
        {
            ReactionTimes = new List<int>();
        }
    }
}
