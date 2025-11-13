using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMushroom : MonoBehaviour
{
    public float jumpForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody _rigidbody = collision.rigidbody;
        Debug.Log("JumpPad hit: " + collision.gameObject.name);

        if (_rigidbody != null)
        {

            _rigidbody.AddForce(Vector3.up * jumpForce * 5f, ForceMode.Impulse);
        }
    }
}
