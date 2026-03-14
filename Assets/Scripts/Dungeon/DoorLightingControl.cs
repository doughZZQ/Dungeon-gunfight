using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }


    public void FadeInDoor(Door door)
    {
        Material variableLitMaterial = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer sr in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(sr, variableLitMaterial));
            }

            isLit = true;
        }
    }


    private IEnumerator FadeInDoorRoutine(SpriteRenderer sr, Material variableLitMaterial)
    {
        sr.material = variableLitMaterial;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            variableLitMaterial.SetFloat("Alpha_Slider", i);

            yield return null;
        }

        sr.material = GameResources.Instance.litMaterial;
    }
}
