using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskSprite : MonoBehaviour
{
    public SpriteRenderer Sprite;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        transform.position = pos;

        transform.localScale = new Vector3(1, 1, 1);

        float width = Sprite.sprite.bounds.size.x;
        float height = Sprite.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
