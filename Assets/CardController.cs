using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState
{
  None,
  Spawned,
  Grabbed,
  Used
}
public class CardController : MonoBehaviour
{
  private CardState state;
  private Vector3 defaultPos;
  private Quaternion defaultRot;
  // Start is called before the first frame update
  void Start()
  {
    state = CardState.Spawned;
    defaultPos = transform.position;
    defaultRot = transform.rotation;
  }

  public void GrabCard()
  {
    state = CardState.Grabbed;
  }
  public void ReleaseCard()
  {
    if (state == CardState.Grabbed)
    {
      state = CardState.None;
      Invoke(nameof(respawnCard), 0.5f);
    }
  }
  public void UseCard()
  {
    state = CardState.Used;
  }
  private void respawnCard()
  {
    if(state == CardState.None)
    {
      transform.position = defaultPos;
      transform.rotation = defaultRot;
    }
  }
  // Update is called once per frame
  void Update()
  {

  }
}
