using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
  public PhysicsButton ButtonToEnable;
  public Elevator Elevator;
  private AudioSource PanelAudio;
  // Start is called before the first frame update
  void Start()
  {
    PanelAudio = GetComponent<AudioSource>();
    if (PanelAudio == null)
      Debug.LogError("Could not find PanelAudio AudioSource");
  }

  // Update is called once per frame
  void Update()
  {

  }
  private void OnTriggerEnter(Collider other)
  {
    if(other.name == "Card")
    {
      if (ButtonToEnable != null && !ButtonToEnable.canPress)
      {
        PanelAudio?.Play();
        ButtonToEnable.EnableButton();
        var Card = other.gameObject.GetComponent<CardController>();
        if (Card != null)
          Card.UseCard();
      }
      else if (Elevator != null)
      {
        PanelAudio?.Play();
        Elevator.Open(false);
      }
    }
  }
}
