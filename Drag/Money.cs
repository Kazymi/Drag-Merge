using System;
using System.Collections;
using System.Collections.Generic;
using EventBusSystem;
using FactoryPool;
using UnityEngine;

public class Money : MonoPooled
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() == false)
        {
            return;
        }
        EventBus.RaiseEvent<IPlayerTakeMoney>(t => t.TakeMoney(transform.position));
        ReturnToPool();
    }
}
