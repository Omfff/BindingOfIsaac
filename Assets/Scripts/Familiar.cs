using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Familiar : MonoBehaviour
{
    private float lastFire;

    private GameObject player;

    public FamiliarData familiar;

    private float lastOffsetX;

    private float lastOffsetY;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float shootHor = Input.GetAxis("ShootHorizontal");
        float shootVer = Input.GetAxis("ShootVertical");
        if ((shootHor != 0 || shootVer != 0) && Time.time > lastFire + familiar.fireDelay)
        {
            Shoot(shootHor, shootVer);
            lastFire = Time.time;
        }

        if(horizontal != 0 || vertical != 0)
        {
            float offsetX = (horizontal < 0) ? Mathf.Floor(horizontal) : Mathf.Ceil(horizontal);
            float offsetY = (vertical < 0) ? Mathf.Floor(vertical) : Mathf.Ceil(vertical);

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, familiar.speed * Time.deltaTime);

            lastOffsetX = offsetX;
            lastOffsetY = offsetY;
        }
        else
        {
            // 啥意思
            if(!(transform.position.x < lastOffsetX +0.5f) || !(transform.position.y < lastOffsetY + 0.5f)){
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x - lastOffsetX,
                    player.transform.position.y - lastOffsetY), familiar.speed * Time.deltaTime);
            }
        }
    }

    void Shoot(float x, float y)
    {
        GameObject bullet = Instantiate(familiar.bulletPrefeb, transform.position, Quaternion.identity) as GameObject;
        float posX = (x < 0) ? Mathf.Floor(x) * familiar.speed : Mathf.Ceil(x) * familiar.speed;
        float posY = (x < 0) ? Mathf.Floor(y) * familiar.speed : Mathf.Ceil(y) * familiar.speed;

        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(posX, posY);
    }
}
