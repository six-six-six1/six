using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPillarEffect : MonoBehaviour
{
    public void Initialize(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
