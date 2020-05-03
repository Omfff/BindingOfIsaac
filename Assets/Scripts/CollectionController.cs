using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;

    public string descirption;

    public Sprite itemImage;
}

public class CollectionController : MonoBehaviour
{
    public Item item;

    public float healthChange;

    public float moveSpeedChange;

    public float attackSpeedChange;

    public float bulletSizeChange;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemImage;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerController.collectedAmount++;
            GameController.MoveSpeedChange(moveSpeedChange);
            GameController.FireRateChange(attackSpeedChange);
            GameController.HealPlayer(healthChange);
            GameController.BulletSizeChange(bulletSizeChange);
            GameController.instance.UpdateCollectedItems(this);

            Destroy(gameObject);
        }

    }
}
