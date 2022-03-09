using UnityEngine;

namespace graph
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        private Mesh mesh;
        public Vector3[] vertices;

        private int[] triangles;

        // Start is called before the first frame update
        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            CreateShape();
        }

        // Update is called once per frame
        void Update()
        {
        }

        void CreateShape()
        {

        }
    }
}