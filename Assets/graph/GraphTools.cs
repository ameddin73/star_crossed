using System;
using System.Collections.Generic;
using System.Linq;
using graph.edge;
using UnityEngine;
using TreeLib;

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
        public static HashSet<Tuple<IEdge, IEdge>> PolygonIntersections(IEnumerable<IEdge> edges)
        {
            // Ordered queue by vector coordinates
            var pointQueue = new SortedSet<Point>();
            // Insert edge vertices into queue
            foreach (var edge in edges)
            {
                pointQueue.Add(new Point(edge.startPosition, edge, null));
                pointQueue.Add(new Point(edge.endPosition, null, edge));
            }

            // Ordered queue of edges under test
            // var edgesUnderTest = new SortedSet<SweepLineEdge>(new SweepLineEdge(null));
            var edgesUnderTest = new AVLTreeRankList<SweepLineEdge>();

            // Results
            var intersections = new HashSet<Tuple<IEdge, IEdge>>();

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

                // Get edge + neighbors
                var edge = new SweepLineEdge(point.start ?? point.end, point.y);
                if (!edgesUnderTest.ContainsKey(edge)) edgesUnderTest.Add(edge);

                SweepLineEdge predecessor = null, successor = null;
                try
                {
                    var edgeEnum = edgesUnderTest.GetFastEnumerable(edge, false);
                    if (edgeEnum.Count() > 1)
                    {
                        predecessor = edgesUnderTest.GetFastEnumerable(edge, false).ElementAt(1).Key;
                        predecessor.sweepLine = point.y;
                    }

                    edgeEnum = edgesUnderTest.GetFastEnumerable(edge);
                    if (edgeEnum.Count() > 1)
                    {
                        successor = edgesUnderTest.GetFastEnumerable(edge).ElementAt(1).Key;
                        successor.sweepLine = point.y;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                // 2. add line if point is start
                if (point.start != null)
                {
                    // 3. Check if this points intersects with its predecessor and successor
                    if (predecessor != null && IntersectionPoint(edge, predecessor) != null)
                    {
                        var intersection = new Tuple<IEdge, IEdge>(predecessor.edge, edge.edge);
                        if (!intersections.Contains(intersection))
                        {
                            // Add intersection point to queue (since neighbors may have switched)
                            pointQueue.Add(new Point((Vector2)IntersectionPoint(edge, predecessor), edge, null));
                            intersections.Add(intersection);
                        }
                    }

                    if (successor != null && IntersectionPoint(edge, successor) != null)
                    {
                        var intersection = new Tuple<IEdge, IEdge>(edge.edge, successor.edge);
                        if (!intersections.Contains(intersection))
                        {
                            pointQueue.Add(new Point((Vector2)IntersectionPoint(edge, successor), null, edge));
                            intersections.Add(intersection);
                        }
                    }
                }
                else
                {
                    // 4. Check if neighbors touch (since we're at end of line)
                    if (predecessor != null && successor != null && IntersectionPoint(predecessor, successor) != null)
                    {
                        var intersection = new Tuple<IEdge, IEdge>(edge, successor);
                        if (!intersections.Contains(intersection))
                        {
                            pointQueue.Add(
                                new Point((Vector2)IntersectionPoint(predecessor.edge, successor.edge), null, null));
                            intersections.Add(intersection);
                        }
                    }

                    // Remove from edges to be tested
                    edgesUnderTest.TryRemove(edge);
                }
            }

            return intersections;
        }

        /// <summary>
        /// https://stackoverflow.com/a/1968345/3434441
        /// </summary>
        /// <param name="s1">first line to test</param>
        /// <param name="s2">second line to test</param> 
        /// <returns>intersection point or null</returns>
        public static Vector2? IntersectionPoint(IEdge s1, IEdge s2)
        {
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
    public class SweepLineEdge : IEdge, IComparable<SweepLineEdge>
    {
        public Vector2 startPosition { get; set; }
        public Vector2 endPosition { get; set; }

        public float sweepLine { get; set; }

        public IEdge edge { get; }

        public SweepLineEdge(IEdge edge, float sweepLine)
        {
            this.startPosition = edge.startPosition;
            this.endPosition = edge.endPosition;
            this.sweepLine = sweepLine;
            this.edge = edge;
        }

        public SweepLineEdge(IEdge edge)
        {
            this.startPosition = edge.startPosition;
            this.endPosition = edge.endPosition;
        }

        /**
         * Returns intersection x-coordinate if intersects, otherwise null
         */
        private float? SweepLineIntersection(float sl)
        {
            // slope
            var m = (endPosition.y - startPosition.y) / (endPosition.x - startPosition.x);
            // y = mx + b   ->    y - mx = b
            var b = startPosition.y - m * startPosition.x;
            // x = (y-b)/m to find x coordinate of sweep-line intersection
            var x = (sl - b) / m;

            // If x is on our line segment
            if ((startPosition.x < x && x < endPosition.x) || (endPosition.x < x && x < startPosition.x) ||
                // We will check edges with approximately since points tend to lie on edges and float equals is iffy
                Mathf.Approximately(startPosition.x, x) || Mathf.Approximately(endPosition.x, x))
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
        public int CompareTo(SweepLineEdge value)
        {
            if (value == null) return 1;
            var xIntersect = SweepLineIntersection(sweepLine);
            var yIntersect = value.SweepLineIntersection(sweepLine);
            if (xIntersect == null) return -1;
            if (yIntersect == null) return 1;

            return ((float)xIntersect).CompareTo(yIntersect);
        }

        public override string ToString()
        {
            return $"{nameof(startPosition)}: {startPosition}, {nameof(endPosition)}: {endPosition}";
        }
    }

    public struct Point : IComparable<Point>
    {
        public Point(Vector2 vector, IEdge start, IEdge end)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.start = start;
            this.end = end;
        }

        public float x { get; set; }
        public float y { get; set; }
        public IEdge start { get; set; }
        public IEdge end { get; set; }

        public int CompareTo(Point other)
        {
            var yComparison = y.CompareTo(other.y);
            return yComparison != 0 ? yComparison : x.CompareTo(other.x);
        }
    }
}