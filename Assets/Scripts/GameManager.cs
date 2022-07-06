using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Light SunLight;
  public GameObject ElevatorButton;
  public Elevator Elevator;
  public GameObject Player;
  public AudioSource Music;
  public bool Upwards;
  public Terrain Terrain;
  public GameObject Water;
  public ElevatorInner ElevatorInner;
  public List<GameObject> Floors;
  public List<GameObject> HandModels;
  public Material HandHighlight;
  public float FloorHeight = 10.0f;
  public float SmoothTime = 0.5f;
  public float Speed = 2;
  public Material HandMaterial;
  private Vector3 ElevatorVelocity;
  private Vector3 PlayerVelocity;
  private Vector3 ElevatorTargetPosition;
  private Vector3 PlayerTargetPosition;
  private bool MoveElevator;
  private bool HasBeenPressed = false;
  private AudioSource ambienceSound;
  private bool cancelCloseDoor = false;
  public List<GameObject> ObjectsToEnableUnderwater;
  public bool ForceHide;
  public bool ForceVisibile;
  public GameObject FloorReplacement;
  public float HideFloorSpeed = 0.1f;
  public List<GameObject> Eyes;
  public float EyeSpeed = 10f;
  private MeshRenderer floorReplacementRenderer;
  private bool activatedUnderwater = false;
  private float currentTransparencyDistance = 30f;
  private float currentSunLightIntensity = 0f;
  // Start is called before the first frame update
  void Start()
  {
    ambienceSound = GetComponent<AudioSource>();
    if (ambienceSound == null)
      Debug.LogError("Missing AudioSource ambienceSound");
    floorReplacementRenderer = FloorReplacement.GetComponent<MeshRenderer>();
  }

  // Update is called once per frame
  void Update()
  {
    Eyes.ForEach(eye =>
    {
      Vector3 relativePos = Player.transform.position - eye.transform.position;
      Quaternion rotation = Quaternion.LookRotation(relativePos);

      Quaternion current = eye.transform.localRotation;

      eye.transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
          * EyeSpeed);
    });
   
    if (ForceHide)
    {
      InvokeRepeating(nameof(reduceFloorVisibility), 0f, 0.05f);
      ForceHide = false;
    }
    if (ForceVisibile)
    {
      currentTransparencyDistance = 100f;
      floorReplacementRenderer.material.SetFloat("_Distance", currentTransparencyDistance);
      ForceVisibile = false;
    }
    if (MoveElevator)
    {
      __SetTargetPositions();
      Elevator.transform.position = Vector3.SmoothDamp(Elevator.transform.position, ElevatorTargetPosition, ref ElevatorVelocity, SmoothTime, Speed);
      Player.transform.position = Vector3.SmoothDamp(Player.transform.position, PlayerTargetPosition, ref PlayerVelocity, SmoothTime, Speed);
      if (!activatedUnderwater && Elevator.transform.position.y < Floors[Floors.Count - 2].transform.position.y - 2)
      {
        activatedUnderwater = true;
        var LastFloor = Floors.LastOrDefault();
        var PreLastFloor = Floors.FirstOrDefault(f => f.name == "Floor -2");
        if (PreLastFloor == null)
          Debug.Log("Coult not find Prelastfloor");
        Floors.Where(x => x != LastFloor && x != PreLastFloor).ToList().ForEach(x => x.SetActive(false));
        //PreLastFloor.transform.Find("Floor").gameObject.SetActive(false);
        PreLastFloor.transform.Find("office_1_ug 1").gameObject.SetActive(false);
        //FloorReplacement.SetActive(true);
        InvokeRepeating(nameof(reduceFloorVisibility), 0f, 0.05f);
        InvokeRepeating(nameof(increaseSunLight), 0f, 0.05f);
        Water.SetActive(true);
        RenderSettings.fog = true;
        //RenderSettings.skybox.SetColor("_Tint", Color.blue);
      }
      if (ElevatorVelocity.magnitude < 0.05)
      {
        MoveElevator = false;
        Elevator.OnDoorOpen += __Elevator_OnDoorOpen;
        Elevator.Open(true);
      }
    }


  }

  private void increaseSunLight()
  {
    if (currentSunLightIntensity >= 1.0f)
      CancelInvoke(nameof(increaseSunLight));
    currentSunLightIntensity += 0.01f;
    SunLight.intensity = currentSunLightIntensity;
  }
  private void reduceFloorVisibility()
  {
    if (currentTransparencyDistance < 0)
      CancelInvoke(nameof(reduceFloorVisibility));
    currentTransparencyDistance -= HideFloorSpeed;
    floorReplacementRenderer.material.SetFloat("_Distance", currentTransparencyDistance);
    if (currentTransparencyDistance > 8)
      RenderSettings.fogEndDistance = currentTransparencyDistance;
  }


  public void OnElevatorButtonPressed()
  {
    if (HasBeenPressed)
      return;
    Debug.Log("GameManager: OnElevatorButtonPressed");
    __SetTargetPositions();
    HasBeenPressed = true;
    StartCoroutine(__StartElevator());

  }
  private void __SetTargetPositions()
  {
    var TargetHeight = Floors.Select(x => x.transform.position.y).Min() - 0.04f;
    var Diff = Elevator.transform.position.y - TargetHeight;
    ElevatorTargetPosition = Elevator.transform.position;
    ElevatorTargetPosition.y = TargetHeight;
    PlayerTargetPosition = Player.transform.position;
    PlayerTargetPosition.y = Player.transform.position.y - Diff;
  }
  public void OnElevatorButtonReleased()
  {
    Debug.Log("Something happened!");
    if (ElevatorButton == null)
      return;
  }
  private IEnumerator __StartElevator()
  {
    yield return new WaitForSeconds(0.5f);
    Debug.Log("__StartElevator");
    Elevator.OnDoorClosed += __MoveElevator;
    Elevator.executeEvents = true;
    Elevator.Close();

  }
  private void __MoveElevator(object sender, EventArgs e)
  {
    MoveElevator = true;
    Music.time = 5f;
    Music.Play();
  }

  private void __Elevator_OnDoorOpen(object sender, EventArgs e)
  {
    if (ObjectsToEnableUnderwater != null)
      ObjectsToEnableUnderwater.ForEach(x => x.SetActive(true));
    ElevatorInner.ElevatorLeft += __ElevatorInner_ElevatorLeft;
    Elevator.OnDoorOpen -= __Elevator_OnDoorOpen;
    Elevator.ElevatorArrivedSound.Play();
    if (HandHighlight != null && HandModels != null && HandModels.Count > 0 && HandModels.Any(x => x != null))
    {
      foreach (var HandModel in HandModels.Where(h => h != null))
      {
        var MeshRender = HandModel.GetComponent<SkinnedMeshRenderer>();
        if (MeshRender != null)
          MeshRender.material = HandHighlight;
      }
    }
  }

  private void __ElevatorInner_ElevatorLeft(object sender, EventArgs e)
  {
    //ElevatorInner.ElevatorEntered += ElevatorInner_ElevatorEnteredUnderwater;
    Elevator.OnDoorClosed -= __MoveElevator;

    Debug.Log("Leaving Elevator and start music");
    // if we reach this point it means we are already underwater when the door opens, so we can start play the sound;
    ambienceSound.volume = 0f;
    ambienceSound.Play();
    InvokeRepeating(nameof(__IncreaseAmbienceVolumne), 0f, 0.1f);
    Invoke(nameof(__CloseElevatorDoorUnderwater), 0.5f);
  }

  private void ElevatorInner_ElevatorEnteredUnderwater(object sender, EventArgs e)
  {
    cancelCloseDoor = true;
  }

  private void __CloseElevatorDoorUnderwater()
  {
    if (!cancelCloseDoor)
      Elevator.Close();
  }

  private void __IncreaseAmbienceVolumne()
  {
    ambienceSound.volume += 0.05f;
    if (ambienceSound.volume >= 1)
      CancelInvoke(nameof(__IncreaseAmbienceVolumne));
    //Debug.Log($"Ambience volumne now {ambienceSound.volume}");
  }
}
