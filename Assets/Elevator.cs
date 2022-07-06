using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
  public GameObject DoorLeft, DoorRight;
  public bool ForceCloseDoor;
  public bool ForceOpenDoor;
  public float DoorSmoothTime = 0.5f;
  public float DoorSpeed = 0.5f;
  public float ScaleSpeed = 21f;
  private Vector3 LeftOpen = new Vector3(0, 1.064f, 1f);
  private Vector3 RightOpen = new Vector3(0, 1.064f, -1f);
  private Vector3 LeftClosed = new Vector3(0, 1.064f, 0);
  private Vector3 RightClosed = new Vector3(0, 1.064f, 0f);
  private bool CloseDoor, OpenDoor;
  private Vector3 DoorLeftVelocity, DoorRightVelocity;
  public AudioSource ElevatorDoorSound;
  public AudioSource ElevatorArrivedSound;
  public event EventHandler OnDoorClosed;
  public event EventHandler OnDoorOpen;
  private float defaultYScale;
  public bool isDoorOpen;
  public bool executeEvents;
  public Material LeftDoorUnderwaterMaterial;
  public Material RightDoorUnderwaterMaterial;
  // Start is called before the first frame update
  void Start()
  {
    defaultYScale = 38f;
    isDoorOpen = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (CloseDoor || ForceCloseDoor)
    {
      DoorLeft.transform.localPosition = Vector3.SmoothDamp(DoorLeft.transform.localPosition, LeftClosed, ref DoorLeftVelocity, DoorSmoothTime, DoorSpeed);
      var LeftScale = DoorLeft.transform.localScale;
      if (LeftScale.y < 115)
        LeftScale.y = LeftScale.y + ScaleSpeed * Time.deltaTime;
      DoorLeft.transform.localScale = LeftScale;
      DoorRight.transform.localPosition = Vector3.SmoothDamp(DoorRight.transform.localPosition, RightClosed, ref DoorRightVelocity, DoorSmoothTime, DoorSpeed);
      var RightScale = DoorRight.transform.localScale;
      if (RightScale.y < 115)
        RightScale.y = RightScale.y + ScaleSpeed * Time.deltaTime;
      DoorRight.transform.localScale = RightScale;

      if (DoorLeftVelocity.magnitude < 0.0001)
      {
        CloseDoor = false;
        ForceCloseDoor = false;
        isDoorOpen = false;
        DoorLeft.GetComponent<MeshRenderer>().material = LeftDoorUnderwaterMaterial;
        DoorRight.GetComponent<MeshRenderer>().material = RightDoorUnderwaterMaterial;
        if (executeEvents)
          OnDoorClosed?.Invoke(this, EventArgs.Empty);
      }
    }
    if (OpenDoor || ForceOpenDoor)
    {
      DoorLeft.transform.localPosition = Vector3.SmoothDamp(DoorLeft.transform.localPosition, LeftOpen, ref DoorLeftVelocity, DoorSmoothTime, DoorSpeed);
      DoorRight.transform.localPosition = Vector3.SmoothDamp(DoorRight.transform.localPosition, RightOpen, ref DoorRightVelocity, DoorSmoothTime, DoorSpeed);
      if (DoorLeftVelocity.magnitude < 0.0001)
      {
        OpenDoor = false;
        ForceOpenDoor = false;
        isDoorOpen = true;
        if (executeEvents)
          OnDoorOpen?.Invoke(this, EventArgs.Empty);
        var LeftScale = DoorLeft.transform.localScale;
        LeftScale.y = defaultYScale;
        DoorLeft.transform.localScale = LeftScale;
        var RightScale = DoorRight.transform.localScale;
        RightScale.y = defaultYScale;
        DoorRight.transform.localScale = RightScale;
      }
    }
  }

  public void Open(bool ExecuteEvents)
  {
    if (isDoorOpen)
      return;
    Debug.Log($"Elevator.Open() with ExecuteEvents {ExecuteEvents}");
    executeEvents = ExecuteEvents;
    OpenDoor = true;
    CloseDoor = false;
    ElevatorDoorSound.Play();
  }
  public void Close()
  {
    CloseDoor = true;
    OpenDoor = false;
    Debug.Log($"Close: ElevatorDoorSound is {(ElevatorDoorSound == null ? "" : "not")} null");
    ElevatorDoorSound.Play();
  }
}
