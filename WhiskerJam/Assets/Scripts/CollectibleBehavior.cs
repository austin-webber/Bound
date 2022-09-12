using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectibleBehavior : MonoBehaviour
{
    [SerializeField] private CanvasGroup collectibleUIGroup;
    [SerializeField] private TextMeshProUGUI collectibleUIText;
    private int totalCollectibles = 4;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip catnipSound;

    private void Start()
    {
        collectibleUIGroup.alpha = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(catnipSound);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            // update text
            Global.collectiblesAcquired += 1;
            collectibleUIText.text = "Catnip: " + Global.collectiblesAcquired + "/" + totalCollectibles;
            // fade UI
            StartCoroutine(FadeInAndOut(gameObject));
        }
    }

    IEnumerator FadeInAndOut(GameObject gameObject)
    {
        // fade UI in
        for (float alpha = 0f; alpha <= 1.05f; alpha += 0.1f)
        {
            collectibleUIGroup.alpha = alpha;
            yield return new WaitForSeconds(.1f);
        }

        // wait 3 seconds
        yield return new WaitForSeconds(3f);

        // fade UI out
        for (float alpha = 1f; alpha >= -0.05f; alpha -= 0.1f)
        {
            collectibleUIGroup.alpha = alpha;
            yield return new WaitForSeconds(.1f);
        }

        gameObject.SetActive(false);
    }

}
