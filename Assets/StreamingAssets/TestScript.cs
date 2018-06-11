using UnityEngine;

// namespace doesnt matter, its here just for clarity
namespace RuntimeCSharpCompiler
{
    // it can even be the same as already existing type, what matters is its in different module
    public class TestLoadScript : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Start");

        }

        void Update()
        {
            Debug.Log("Update");
        }
    }
}