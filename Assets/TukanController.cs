using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TukanController : MonoBehaviour
{
  public Transform TucanPositions;
  public float speed = 3f;
  public float smoothTime = 2f;
  public float rotateSpeed = 5f;
  private Animator Animator;
  private bool isInFlight;
  private bool isInRotation;
  private Transform targetPoint;
  public int posIndex = 0;
  private Vector3 velocity;

  private void OnTriggerEnter(Collider other)
  {
    if (isInFlight || posIndex == TucanPositions.childCount - 1) 
      return;
    isInRotation = false;
    Debug.Log("Tukan Trigger entered");
    posIndex++;
    targetPoint = TucanPositions.GetChild(posIndex);
    isInFlight = true;
    Animator.SetTrigger("TucanMove");
  }
  // Start is called before the first frame update
  void Start()
  {
    Animator = GetComponent<Animator>();
    if (Animator == null)
      Debug.LogError("Animator not found");
  }

  // Update is called once per frame
  void Update()
  {
    if (isInFlight)
    {
      transform.position = Vector3.SmoothDamp(transform.position, targetPoint.position, ref velocity, smoothTime, speed);
      var direction = targetPoint.position - transform.position;

      //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * Time.deltaTime);
      if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
      {
        isInRotation = true;
        Debug.Log("Tukan in rotation");
      }
      if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
      {
        Debug.Log("Tukan reached target position");
        isInFlight = false;
        Animator.SetTrigger("TucanIdle");
      }
    }
    if (isInRotation)
    {
      transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, rotateSpeed * Time.deltaTime);
      if (Quaternion.Angle(transform.rotation, targetPoint.rotation) <= 0.01f)
      {
        isInRotation = false;
        Debug.Log("Tukan reached targetrotation");
      }
    }
  }
}
