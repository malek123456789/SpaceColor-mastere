using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjects : MonoBehaviour {

    void OnBecameInvisible()
    {
        // Wait 2 second, as gameObjects are removed too quickly (E.g player can sometimes see them disappear).
        try
        {
            if(gameObject.activeInHierarchy)
                StartCoroutine(Wait());
        }
        catch {}
    }

    IEnumerator Wait()
    {
        //yield on a new YieldInstruction that waits for 2 seconds, then destroy gameObject.
        yield return new WaitForSeconds(2);

        try
        {
            Destroy(gameObject);
        }
        catch{}
    }
}
