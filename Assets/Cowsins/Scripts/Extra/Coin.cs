using UnityEngine;
using System.Collections;

namespace cowsins
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int minCoins, maxCoins;
        [SerializeField] private AudioClip collectCoinSFX;
        [SerializeField] private float recollectionTime = 5.0f; // Time before the coin can be recollected
        private Renderer[] renderers;
        private Collider coinCollider;

        private void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
            coinCollider = GetComponent<Collider>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning("No Renderer components found in the Coin game object or its children.");
            }

            if (coinCollider == null)
            {
                Debug.LogWarning("Collider component is missing from the Coin game object.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            int amountOfCoins = Random.Range(minCoins, maxCoins);
            CoinManager.Instance.AddCoins(amountOfCoins);
            UIController.instance.UpdateCoinsPanel();
            UIEvents.onCoinsChange?.Invoke(CoinManager.Instance.coins);
            SoundManager.Instance.PlaySound(collectCoinSFX, 0, 1, false, 0);

            StartCoroutine(RecollectCoin());
        }

        private IEnumerator RecollectCoin()
        {
            // Make the coin invisible and disable its collider
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            if (coinCollider != null)
            {
                coinCollider.enabled = false;
            }

            // Wait for the specified recollection time
            yield return new WaitForSeconds(recollectionTime);

            // Make the coin visible again and enable its collider
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
            if (coinCollider != null)
            {
                coinCollider.enabled = true;
            }
        }
    }
}
