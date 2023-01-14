using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResource : MonoBehaviour
{
    public int health;

    public int wood;
    public int stone;
    public int magic;

    public GameObject fx_destroy;

    bool isWobbling = false;

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Singleton.wood += wood;
            GameManager.Singleton.stone += stone;
            GameManager.Singleton.magic += magic;

            Instantiate(fx_destroy, transform.position + Vector3.up, Quaternion.identity, null);
            Destroy(gameObject);
        }
        else if(!isWobbling)
        {
            StartCoroutine(Wobble());
        }
    }

    IEnumerator Wobble()
    {
        isWobbling = true;
        Vector3 originPosition = gameObject.transform.position;
        Vector3 direction = Camera.main.transform.right;

        for (float distance = 0f; distance <= 3f; distance += 0.05f)
        {
            gameObject.transform.position = originPosition + Mathf.Sin(distance * Mathf.PI) * 0.1f * direction;
            yield return new WaitForEndOfFrame();
        }
        gameObject.transform.position = originPosition;
        isWobbling = false;
    }
}
