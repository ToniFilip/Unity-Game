using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCollector : MonoBehaviour
{
    private List<Rigidbody> magicBalls = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        // Register new magic ball
        if (other.gameObject.layer == MyLayer.magicLayer)
        {
            magicBalls.Add(other.GetComponent<Rigidbody>());
        }
    }

    private void FixedUpdate()
    {
        for (int k = magicBalls.Count - 1; k >= 0; k--) 
        {
            // Check if ball still exists
            if (magicBalls[k] == null) magicBalls.RemoveAt(k);
            else
            {
                Vector3 distance = transform.position - magicBalls[k].transform.position;
                float distanceSqr = distance.sqrMagnitude;

                // Check if ball is in collect range
                if (distanceSqr < 2f)
                {
                    // Add magic to player and destroy ball
                    SoundController.Singleton.playSoundeffect(SoundController.Singleton.pickUpMagic, GameManager.Singleton.player.audioSource, 0.2f);
                    Destroy(magicBalls[k].gameObject);
                    magicBalls.RemoveAt(k);
                    GameManager.Singleton.magic += 1;
                } else
                {
                    // Otherwise add force to ball towards player
                    magicBalls[k].AddForce(distance / distanceSqr * 25f);
                }                
            }
        }
    }
}
