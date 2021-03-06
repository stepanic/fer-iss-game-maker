﻿using System;
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

        public double AverageReactionTime
        {
            get
            {
                if (ReactionTimes.Count == 0) return 0;
                return Math.Round(ReactionTimes.Average(), 2);
            }
        }


        public GoNoGoResult()
        {
            ReactionTimes = new List<int>();
        }
    }
}
