using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitmarker : MonoBehaviour
{
    public bool validtarget = false;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        Physics2D.IgnoreLayerCollision(6, 10);

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Cards")& collision.collider.gameObject.GetComponent<CardDisplay>().isMine==false)
        {
            validtarget = true;
        }
     

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        validtarget = false;
    }

}
