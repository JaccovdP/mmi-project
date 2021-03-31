using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Target
    {
        public Point center;
        public float radius;

        public Target(Point center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool GoalReached(Point currentPos)
        {
            float dist = Distance(center,currentPos); //Distance between currentPos and center

            if (dist <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        float Distance(Point a, Point b)
        {
            return (float)Math.Sqrt(Math.Pow(b.x - a.x, 2) + Math.Pow(b.y - a.y, 2));
        }
    }
}
