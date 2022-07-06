using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
  private Collider[] handColliders;
  public Transform target;
  public Renderer nonPhysicalHand;
  public float showNonPhysicalHandDistance = 0.05f;
  public CardController Card;
  private Rigidbody rb;
  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    handColliders = GetComponentsInChildren<Collider>();
  }

  private void Update()
  {
    float distance = Vector3.Distance(transform.position, target.position);
    if (distance > showNonPhysicalHandDistance)
    {
      nonPhysicalHand.enabled = true;
    }
    else
      nonPhysicalHand.enabled = false;
  }

  void FixedUpdate()
  {
    rb.velocity = (target.position - transform.position)/Time.fixedDeltaTime;
    Quaternion rotDiff = target.rotation * Quaternion.Inverse(transform.rotation);
    rotDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
    Vector3 rotDiffInDegree = angleInDegree * rotationAxis;
    rb.angularVelocity = (rotDiffInDegree * Mathf.Deg2Rad/Time.fixedDeltaTime);
  }
  public void EnableCollider()
  {
    Invoke(nameof(EnableColliders), 0.5f);
    Card.ReleaseCard();
  }
  private void EnableColliders()
  {
    foreach (var item in handColliders)
    {
      item.enabled = true;
    }
  }

  public void DisableCollider()
  {
    foreach (var item in handColliders)
    {
      item.enabled = false;
    }
    Card.GrabCard();
  }
}
