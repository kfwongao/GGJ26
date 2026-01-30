using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public bool b = true;
    public Image image;
    public float speed = 0.5f;

    public float countDownTime = 0f;
    public float coolDownTime = 0f;
    public bool canSpawnSkill = true;

    public Text progress;

    public Image skillIconImg;
    public Sprite[] skillSprites;
    public int skillid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        skillIconImg.sprite = skillSprites[PlayerData.Instance.GetSkillIndex(skillid)];
        if (b)
        {
            if(countDownTime > 0)
            {
                canSpawnSkill = false;
                countDownTime -= Time.deltaTime;
                progress.enabled = true;
            }
            image.fillAmount = countDownTime / coolDownTime;
            if (progress)
            {
                progress.text = (int)Mathf.Ceil(countDownTime) + "";
            }

            if (countDownTime <= 0)
            {
                image.fillAmount = 0;
                canSpawnSkill = true;
                countDownTime = 0;
                progress.enabled = false;
            }
        }
    }

    public void StartCoolDown(float time)
    {
        coolDownTime = time;
        countDownTime = time;
        if(time > 0)
        {
            canSpawnSkill = false;
        } else
        {
            canSpawnSkill = true;
        }

    }
}
