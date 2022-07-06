using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerScript : MonoBehaviour
{
  public Material PlantMaterial;
  public Shader PlantTransitionShader;
  public Color TargetColor;
  public float TransitionSpeed = 10f;
  
  public void StartTransition()
  {
    InvokeRepeating(nameof(LerpColor), 0f, Time.deltaTime);
  }
  private void LerpColor()
  {
    PlantMaterial.SetColor("_Color", Color.Lerp(TargetColor, PlantMaterial.color, TransitionSpeed * Time.deltaTime));
    if (PlantMaterial.color == TargetColor)
    {
      CancelInvoke(nameof(LerpColor));
      PlantMaterial.shader = PlantTransitionShader;
    }
  }
}
