using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    public float bounce = 20f;
    
    private void OnCollisionEnter2D(Collision2D collision2D) {
        if (collision2D.gameObject.CompareTag("Player")) {
            collision2D.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
}
