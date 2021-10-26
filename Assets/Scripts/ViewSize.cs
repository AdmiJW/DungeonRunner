
using UnityEngine;

public class ViewSize : MonoBehaviour
{
    public Vector3 size;

    // Start is called before the first frame update
    void Awake()
    {
        size = GetComponent<Renderer>().bounds.size;
    }
}
