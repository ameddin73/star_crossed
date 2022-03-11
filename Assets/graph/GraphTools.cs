using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using graph.edge;
using UnityEngine;

namespace graph
{
    public static class GraphTools
    {
        /// <summary>
        /// Takes a list of edges and detects any intersections INCLUDING SHARED
        /// VERTICES
        ///
        /// </summary>
        /// <param name="edges">List of IEdges representing line segmens</param>
        /// <returns>List of intersections</returns>
        public static IEdge[][] PolygonIntersections(IEdge[] edges)
        {
            // Ordered queue by vector coordinates
            var pointQueue = new SortedSet<Point>(new Point());
            // Insert edge vertices into queue
            foreach (var edge in edges)
            {
                pointQueue.Add(new Point(edge.startPosition, edge, true));
                pointQueue.Add(new Point(edge.endPosition, edge, false));
            }

            // Ordered queue of edges under test
            var edgesUnderTest = new SortedSet<SweepLineEdge>(new SweepLineEdge(null));

            // Results
            var intersections = new IEdge[][] { };

            /********************************* Sweep-line Algorithm *********************************/
            // 1. pop through points using y-value as sweep-line
            // 2. add edges that start w/ this point
            // 3. compare un-compared neighbors
            // 4. add intersections to point queue
            // https://www.geeksforgeeks.org/given-a-set-of-line-segments-find-if-any-two-segments-intersect/
            while (pointQueue.Any())
            {
                // 1. pop point
                var point = pointQueue.First();
                pointQueue.Remove(point);

                // 2. add line if point is start
                if (point.start)
                {
                    var edge = (SweepLineEdge) point.edge;
                    edgesUnderTest.Add(edge);

                    // 3. Check if this points intersects with its predecessor and successor
                    // var select = edgesUnderTest.V
                    // edgesUnderTest.ElementAt(3);
                }

                // 3. compare

                // TODO figure the rest of this out
                // https://youtu.be/I9EsN2DTnN8?t=229
            }

            return intersections;
        }

        public static Vector2? IntersectionPoint(IEdge s1, IEdge s2)
        {
            // https://stackoverflow.com/a/1968345/3434441
            var s1_x = s1.endPosition.x - s1.startPosition.x;
            var s1_y = s1.endPosition.y - s1.startPosition.y;
            var s2_x = s2.endPosition.x - s2.startPosition.x;
            var s2_y = s2.endPosition.y - s2.startPosition.y;

            var s = (-s1_y * (s1.startPosition.x - s2.startPosition.x) +
                     s1_x * (s1.startPosition.y - s2.startPosition.y)) /
                    (-s2_x * s1_y + s1_x * s2_y);
            var t =
                (s2_x * (s1.startPosition.y - s2.startPosition.y) - s2_y * (s1.startPosition.x - s2.startPosition.x)) /
                (-s2_x * s1_y + s1_x * s2_y);

            if (!(s >= 0) || !(s <= 1) || !(t >= 0) || !(t <= 1)) return null;
            // Collision detected
            var x = s1.startPosition.x + (t * s1_x);
            var y = s1.startPosition.y + (t * s1_y);
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// IEdge implementation for use in edge set intersection testing using
    /// the sweep-line algorithm. Holds a float representing the sweep-line's
    /// y-coordinate and can report its intersection point
    /// </summary>
    public class SweepLineEdge : IEdge, IComparer<SweepLineEdge>
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
            var m = (endPosition.y - startPosition.y) / (endPosition.x - startPosition.x);
            // y = mx + b   ->    y - mx = b
            var b = startPosition.y - m * startPosition.x;
            // x = (y-b)/m to find x coordinate of sweep-line intersection
            var x = (sweepLine - b) / m;

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
            throw new NotImplementedException("I don't have a GameObject, feel free to delete me!");
        }

        /// <summary>
        /// Compare x-coordinate of intersection point with sweep-line of each edge
        /// </summary>
        public int Compare(SweepLineEdge x, SweepLineEdge y)
        {
            var xIntersect = x.SweepLineIntersection();
            var yIntersect = y.SweepLineIntersection();
            if (xIntersect == null || yIntersect == null)
                throw new ArgumentException("Edge does not intersect sweep-line!");

            return ((float) xIntersect).CompareTo(yIntersect);
        }
    }

    public struct Point : IComparer<Point>
    {
        public Point(Vector2 vector, IEdge edge, bool start)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.edge = edge;
            this.start = start;
        }

        public float x { get; set; }
        public float y { get; set; }
        public IEdge edge { get; set; }

        // True if start point for the intersection algo
        // false if endpoint (bottom)
        public bool start { get; set; }

        public int Compare(Point x, Point y)
        {
            var yComparison = x.y.CompareTo(y.y);
            return yComparison != 0 ? yComparison : x.x.CompareTo(y.x);
        }
    }
}