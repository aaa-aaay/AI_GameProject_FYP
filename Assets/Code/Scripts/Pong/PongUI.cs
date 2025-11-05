using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[Serializable]
public class PowerUpUI
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform transform;
    [SerializeField] private GameObject owner;

    public Animator GetAnimator()
    {
        return animator;
    }

    public GameObject GetOwner()
    {
        return owner;
    }

    public void SetOwner(GameObject new_owner)
    {
        owner = new_owner;
    }

    public RectTransform GetTransform()
    {
        return transform;
    }
}

[Serializable]
public enum BuffType
{
    Shrink = 1,
    Wall,
    Slow
}

[Serializable]
public class NumberToImage
{
    public int value;
    public Sprite image;
}

public class PongUI : MonoBehaviour
{
    public static PongUI instance;

    [SerializeField] private List<PowerUpUI> powers;
    [SerializeField] private List<NumberToImage> images;

    private int count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            count = 0;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public int GetActiveCount()
    {
        return count;
    }

    public void AddCount()
    {
        count++;
    }

    public void UpdateSlider(GameObject owner, float value)
    {
        for (int i = 0; i < powers.Count; i++)
        {
            if (powers[i].GetOwner() == owner)
            {
                if (value == 0)
                {
                    powers[i].GetAnimator().gameObject.SetActive(false);
                    count -= 1;
                }

                break;
            }
        }
    }

    public void CreateNew(GameObject owner, BuffType type)
    {
        PowerUpUI ui = GetUnused();

        if (ui != null)
        {
            ui.GetAnimator().gameObject.SetActive(true);
            ui.GetAnimator().SetInteger("Type", (int) type);
            ui.SetOwner(owner);
            Transform parent = ui.GetTransform().parent;
            ui.GetTransform().SetParent(null);
            ui.GetTransform().SetParent(parent);
        }
    }

    public Sprite GetSpriteByValue(int number)
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (number == images[i].value)
            {
                return images[i].image;
            } 
        }

        return images[0].image;
    }

    public PowerUpUI GetUnused()
    {
        for (int i = 0; i < powers.Count; i++)
        {
            if (!powers[i].GetAnimator().gameObject.activeSelf)
            {
                return powers[i];
            }
        }

        return null;
    }
}
