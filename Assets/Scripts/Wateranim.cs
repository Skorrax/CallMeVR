using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wateranim : MonoBehaviour
{

	public float fps = 30.0f; // FPS of the projection
	public Texture2D[] frames;

	private int frame;
	private Projector projector;
    void Start()
	{
		projector = GetComponent<Projector>();
		NextFrame();
		InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
	}

	void NextFrame()
	{
		projector.material.SetTexture("_ShadowTex", frames[frame]);
		frame = (frame + 1) % frames.Length;
	}

}
