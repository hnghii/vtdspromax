using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    private PlayerManager pmanager;
    private Animator anim;
    public Image bloodtop;
    public Image bloodbot;
    public Image bloodleft;
    public Image bloodright;
    public float blinkDuration = 1f; // Thời gian chóp chóp
    public float blinkSpeed = 0.1f; // Tốc độ chóp chóp
    public GameObject diePanel;

    void Start()
    {
        bloodtop.enabled=false;
        bloodbot.enabled=false;
        bloodleft.enabled=false;
        bloodright.enabled=false;
        anim = GetComponent<Animator>();
        pmanager = GetComponent<PlayerManager>();
        if (pmanager != null)
        {
            maxHealth = pmanager.maxHP; // Cập nhật maxHealth từ PlayerManager
            currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogError("PlayerManager component not found on " + gameObject.name);
        }
    }

    private void Update()
    {
        maxHealth = pmanager.maxHP;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(BlinkScreenDamage());
        currentHealth -= damage;
        healthSlider.value = currentHealth; // Cập nhật thanh máu
        if (currentHealth > 0)
        {
            bloodtop.enabled = true;
            bloodbot.enabled = true;
            bloodleft.enabled = true;
            bloodright.enabled = true;
            StartCoroutine(FadeOut(bloodtop));
            StartCoroutine(FadeOut(bloodbot));
            StartCoroutine(FadeOut(bloodleft));
            StartCoroutine(FadeOut(bloodright));
            Invoke("HideScreenDamage", 0.5f);
            FindAnyObjectByType<AudioManager>().Play("Hurt");
            FindAnyObjectByType<AudioManager>().Play("Hurt1");
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (!pmanager.isDead) { FindAnyObjectByType<AudioManager>().PlayButWait("Dead"); }
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthSlider.value = currentHealth; // Cập nhật thanh máu
    }

    void Die()
    {
        pmanager.isDead = true;
        anim.SetTrigger("isdead");
        FindAnyObjectByType<Spawner>().Stop = true;
        if (PlayerPrefs.GetInt("highScore") != null)
        {
            int oldScore = PlayerPrefs.GetInt("highScore");
            if (oldScore < GameObject.Find("SPAWNER").GetComponent<Spawner>().wave-1)
            {
                PlayerPrefs.SetInt("highScore", GameObject.Find("SPAWNER").GetComponent<Spawner>().wave-1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("highScore", GetComponent<Spawner>().wave-1);
        }
        StartCoroutine(showDiePanel());
    }
    void HideScreenDamage()
    {
       bloodtop.enabled=false;
        bloodbot.enabled=false;
        bloodleft.enabled=false;
        bloodright.enabled=false;
    }
    IEnumerator BlinkScreenDamage()
    {
        float endTime = Time.time + blinkDuration;
        while (Time.time < endTime)
        {
            bloodtop.enabled = !bloodtop.enabled;
            bloodbot.enabled = !bloodbot.enabled;
            bloodleft.enabled = !bloodleft.enabled;
            bloodright.enabled = !bloodright.enabled; // Đảo trạng thái hiển thị của Image
            yield return new WaitForSeconds(blinkSpeed); // Đợi một khoảng thời gian trước khi chóp tiếp
        }
        bloodtop.enabled = false;
        bloodbot.enabled = false;
        bloodleft.enabled = false;
        bloodright.enabled = false; // Đảm bảo Image được tắt sau khi chóp xong
    }

    IEnumerator FadeOut(Image blood)
    {
        float elapsedTime = 0f;
        Color originalColor = blood.color;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1); // Fade out
            blood.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    IEnumerator FadeIn(Image blood)
    {
        float elapsedTime = 0f;
        Color originalColor = blood.color;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 1); // Fade out
            blood.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    IEnumerator showDiePanel()
    {
        
        yield return new WaitForSeconds(5);
        diePanel.SetActive(true);
    }




}