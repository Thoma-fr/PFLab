using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEntity
{
    public float life { get; set; }
    public void TakeDamage(float damage);
}
