using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
  [SerializeField] private float threshold = 0.1f;
  [SerializeField] private float deadZone = 0.025f;
  [SerializeField] public bool canPress;
  [SerializeField] private GameObject clicker;
  [SerializeField] public Material DisabledMaterial;
  [SerializeField] public Material EnabledMaterial;
  public AudioSource PressedDisabledSound;
  public AudioSource PressedEnabledSound;
  private bool isPhysicalPressed;
  private bool isPressed;
  private Vector3 startPos;
  private ConfigurableJoint joint;
  public UnityEvent onPressed, onReleased;
  public GameObject Plane;
  // Start is called before the first frame update
  void Start()
  {
    startPos = transform.localPosition;
    joint = GetComponent<ConfigurableJoint>();    
    
  }

  // Update is called once per frame
  void Update()
  {
    if (!isPressed && GetValue() + threshold >= 1)
      Pressed();
    if (isPhysicalPressed && GetValue() - threshold <= 0)
      Released();
  }

  private float GetValue()
  {
    var Value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
    if (Math.Abs(Value) < deadZone)
      Value = 0;
    return Mathf.Clamp(Value, -1f, 1f);
  }
  private void Pressed()
  {
    //Debug.Log($"Pressed triggered with isphysicalpressed = {isPhysicalPressed} and ispressed = {isPressed}");
    if (isPhysicalPressed)
      return;
    isPhysicalPressed = true;
    if (!canPress)
    {
      Debug.Log("Playing disabled sound");
      PressedDisabledSound?.Play();
      return;
    }
    PressedEnabledSound?.Play();
    isPressed = true;
    
    onPressed.Invoke();
    Debug.Log("Pressed");
  }
  private void Released()
  {
    isPhysicalPressed = false;
    isPressed = false;
    if (!canPress)
      return;
    disableButton();
    onReleased.Invoke();
    Debug.Log("Released");
  }
  public void EnableButton()
  {
    if (Plane != null)
    {
      canPress = true;
      var MeshRender = Plane.GetComponent<MeshRenderer>();
      if (MeshRender != null)
      {
        var Mats = MeshRender.materials;
        Mats[2] = EnabledMaterial;
        MeshRender.materials = Mats;
      }
    }
  }
  private void disableButton()
  {
    if (Plane != null)
    {
      canPress = false;
      var MeshRender = Plane.GetComponent<MeshRenderer>();
      if (MeshRender != null)
      {
        var Mats = MeshRender.materials;
        Mats[2] = DisabledMaterial;
        MeshRender.materials = Mats;
      }
    }
  }
}
