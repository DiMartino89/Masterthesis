using UnityEngine;

public class PresentationBuild : MonoBehaviour
{
    private GameObject _cactusObject;

    private void Start()
    {
        var cactusPrefab = Resources.Load("Cactus/Prefab/Cactus") as GameObject;
        _cactusObject = GameObject.Instantiate(cactusPrefab);
    }

    private void Update()
    {
	//_cactusObject.transform.Rotate(Vector3.up);
    }

    // This method will destroy the cube when we delet the component 
    private void OnDestroy()
    {
        if (_cactusObject != null)
        {
            Destroy(_cactusObject);
        }
    }
}
