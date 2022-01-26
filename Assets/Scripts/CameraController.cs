using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
  [SerializeField]
    private float speed = 15f;
  [SerializeField]
    private float mouseCapSize = 30f;
  [SerializeField]
    private Bounds bounds;
  [SerializeField]
    private Vector3 direction;

    void Awake()
    {
      direction = new Vector3();
    }

    // Start is called before the first frame update
    void Start()
    {
        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        Tilemap T = GameObject.FindWithTag("BgTilemap").GetComponent<Tilemap>();
        T.CompressBounds();
        bounds = T.localBounds;
        bounds.max = bounds.max - new Vector3(horzExtent,vertExtent,10);
        bounds.min = bounds.min + new Vector3(horzExtent,vertExtent,-10);
        transform.position = bounds.min;
    }

    // Update is called once per frame
    void Update()
    {
        direction = Vector3.zero;
        if (Input.mousePosition.x < mouseCapSize) direction += Vector3.left;
        if (Input.mousePosition.x > Screen.width - mouseCapSize) direction += Vector3.right;
        if (Input.mousePosition.y < mouseCapSize) direction += Vector3.down;
        if (Input.mousePosition.y > Screen.height - mouseCapSize) direction += Vector3.up;

        Vector3 newPos = transform.position + direction * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, bounds.min.x, bounds.max.x);
        newPos.y = Mathf.Clamp(newPos.y, bounds.min.y, bounds.max.y);
        transform.position = newPos;
    }
}
