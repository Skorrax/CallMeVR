using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInner : MonoBehaviour
{
  public event EventHandler ElevatorEntered;
  public event EventHandler ElevatorLeft;

  private void OnTriggerEnter(Collider other)
  {
    if (other.name != "XR Origin" || !(other is CapsuleCollider))
      return;
    Debug.Log($"OnElevatorTrigger entered  by {other.name}");
    ElevatorEntered?.Invoke(this, EventArgs.Empty);
  }
  private void OnTriggerExit(Collider other)
  {
    if (other.name != "XR Origin" || !(other is CapsuleCollider))
      return;
    Debug.Log($"OnElevatorTrigger exit  by {other.name}");
    ElevatorLeft?.Invoke(this, EventArgs.Empty);
  }
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
