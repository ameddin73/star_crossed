using UnityEngine;

namespace graph.edge
{
    public interface IEdge
    {
        Vector2 startPosition { get; set; }
        Vector2 endPosition { get; set; }

        public void Destroy();
    }
}