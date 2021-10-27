using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + cameraOffset.x, 
                                        transform.position.y, 
                                        player.transform.position.z + cameraOffset.z);
    }
}
