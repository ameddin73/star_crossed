using System;
using System.Collections.Generic;
using graph.edge;
using UnityEngine;

namespace graph
{
    public static class GraphTools
    {
        public static IEdge[][] Intersections(IEdge[] edges)
        {
            // Ordered queue by vector coordinates
            var pointQueue = new SortedSet<Vector2>(new Vector2Comparer());
            // ordered queue by edge intersection w/ sweepline
            var sweepLine = new SortedSet<SweepLineEdge>(new SweepLineEdgeComparer());

            // Results
            var intersections = new IEdge[][] { };

            // Insert edge vertices into queue
            foreach (var edge in edges)
            {
                if (!pointQueue.Contains(edge.startPosition)) pointQueue.Add(edge.startPosition);
                if (!pointQueue.Contains(edge.endPosition)) pointQueue.Add(edge.endPosition);
            }

            // TODO

            return intersections;
        }

        public class Vecto
        {
            public Vecto(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public float x { get; set; }
            public float y { get; set; }
        }

        public static Vector2? IntersectionPoint(IEdge s1, IEdge s2)
        {
            // I copied this from the internet, that's why it's weird
            // https://stackoverflow.com/a/1968345/3434441
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = s1.endPosition.x - s1.startPosition.x;
            s1_y = s1.endPosition.y - s1.startPosition.y;
            s2_x = s2.endPosition.x - s2.startPosition.x;
            s2_y = s2.endPosition.y - s2.startPosition.y;

            float s, t;
            s = (-s1_y * (s1.startPosition.x - s2.startPosition.x) + s1_x * (s1.startPosition.y - s2.startPosition.y)) /
                (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (s1.startPosition.y - s2.startPosition.y) - s2_y * (s1.startPosition.x - s2.startPosition.x)) /
                (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                var x = s1.startPosition.x + (t * s1_x);
                var y = s1.startPosition.y + (t * s1_y);
                return new Vector2(x, y);
            }

            return null;
        }
    }

    public class Vector2Comparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            var yComparison = x.y.CompareTo(y.y);
            if (yComparison != 0) return yComparison;
            return x.x.CompareTo(y.x);
        }
    }

    public class SweepLineEdgeComparer : IComparer<SweepLineEdge>
    {
        public int Compare(SweepLineEdge x, SweepLineEdge y)
        {
            float? xIntersect = x.SweepLineIntersection();
            float? yIntersect = y.SweepLineIntersection();
            if (xIntersect == null || yIntersect == null)
                throw new ArgumentException("Edge does not intersect sweepline!");

            return ((float) xIntersect).CompareTo(yIntersect);
        }
    }

    public class SweepLineEdge : IEdge
    {
        public Vector2 startPosition { get; set; }
        public Vector2 endPosition { get; set; }

        public float sweepLine { get; set; }

        public SweepLineEdge(IEdge edge, float sweepLine)
        {
            this.startPosition = edge.startPosition;
            this.endPosition = edge.endPosition;
            this.sweepLine = sweepLine;
        }

        public SweepLineEdge(IEdge edge)
        {
            this.startPosition = edge.startPosition;
            this.endPosition = edge.endPosition;
        }

        /**
         * Returns true with intersection point if intersects, otherwise false with 0
         */
        public float? SweepLineIntersection()
        {
            // slope
            float m = (endPosition.y - startPosition.y) / (endPosition.x - startPosition.x);
            // y = mx + b   ->    y - mx = b
            float b = startPosition.y - m * startPosition.x;
            // x = (y-b)/m to find x coordinate of sweepline intersection
            float x = (sweepLine - b) / m;

            // If x is on our line segment
            if ((startPosition.x <= x && x <= endPosition.x) || (endPosition.x <= x && x <= endPosition.x))
            {
                return x;
            }
            else
            {
                return null;
            }
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}