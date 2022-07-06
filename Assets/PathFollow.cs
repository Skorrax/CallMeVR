using UnityEngine;
using System.Collections;

public class PathFollow : MonoBehaviour
{

  public float speed = 3f;
  public float rotateSpeed = 5f;
  public Transform pathGroup;
  public bool TestTrigger;
  private Transform targetPoint;
  private int index;
  private Vector3 velocity;
  public float degrees = 45f;
  public float radius = 1f;
  private bool onSpeed;
  void OnDrawGizmos()
  {
    Vector3 from;
    Vector3 to;
    for (int a = 0; a < pathGroup.childCount; a++)
    {
      from = pathGroup.GetChild(a).position;
      to = pathGroup.GetChild((a + 1) % pathGroup.childCount).position;
      Gizmos.color = new Color(1, 0, 0);
      Gizmos.DrawLine(from, to);
    }
  }
  private void OnTriggerEnter(Collider other)
  {
    if (onSpeed)
      return;
    //var direction = targetPoint.position - transform.position;
    //direction = direction.normalized;
    //Vector3 axis = Vector3.Cross(direction, Vector3.up);
    //var rot = Quaternion.AngleAxis(degrees, axis) * Vector3.right;
    //var newObject = new GameObject();
    //newObject.transform.position = transform.position + rot * radius;
    //targetPoint = newObject.transform;

    //var radians = degrees * Mathf.Deg2Rad;
    //var x = Mathf.Cos(radians);
    //var z = Mathf.Sin(radians);
    //var newOffset = new Vector3(x, 0, z) * radius;
    //targetPoint.position = transform.position + newOffset;
    speed *= 2;
    onSpeed = true;
    Invoke(nameof(reduceSpeed), 2f);
  }
  void Start()
  {
    index = 0;
    targetPoint = pathGroup.GetChild(index);
  }

  // Update is called once per frame
  void Update()
  {
    if (TestTrigger)
    {
      TestTrigger = false;
      OnTriggerEnter(null);
    }
    transform.position = Vector3.SmoothDamp(transform.position, targetPoint.position, ref velocity, 0f, speed);
    var direction = targetPoint.position - transform.position;
    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * Time.deltaTime);
    if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
    {
      index++;
      index %= pathGroup.childCount;
      targetPoint = pathGroup.GetChild(index);
      //transform.LookAt(targetPoint);
    }
  }
  private void reduceSpeed()
  {
    speed /= 2;
    onSpeed = false;
  }
}