using UnityEngine;
using System.Collections;
using TMPro;

namespace cowsins
{
    public class Lootbox : Interactable
    {
        [SerializeField] private GameObject[] loot;

        [SerializeField, Min(0)] private int price;

        [SerializeField] private float delayToReceiveLoot;

        [SerializeField] private float minSpawnAngle, maxSpawnAngle, spawnDistance;

        [SerializeField] private bool directionDependsOnPlayerPosition;

        [SerializeField] private float[] respawnTimes; // Array of times to wait before respawning the lootbox

        [SerializeField] private TextMeshProUGUI countdownText; // TextMeshProUGUI for displaying the countdown

        private Animation anim;

        private AudioSource audioSource;

        private string baseInteractText;

        private bool isRespawning = false; // Flag to indicate if the lootbox is respawning

        private int respawnIndex = 0; // Index for the current respawn time

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        private void Start()
        {
            anim = GetComponent<Animation>();
            audioSource = GetComponent<AudioSource>();
            baseInteractText = "Collectible";

            // Save the initial state
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialScale = transform.localScale;

            SetInteractText();
            Debug.Log($"Initial interactText: {interactText}");
        }

        private void SetInteractText()
        {
            if (isRespawning)
            {
                interactText = "Wait";
            }
            else if (price != 0 && !CoinManager.Instance.CheckIfEnoughCoins(price))
            {
                interactText = $"Not collectible [{price}]";
            }
            else
            {
                interactText = $"Collectible [{price}]";
            }

            // Ensure TextMeshPro is updated and active
            if (countdownText != null)
            {
                countdownText.text = interactText;
                countdownText.gameObject.SetActive(true);
            }
        }

        public override void Highlight()
        {
            SetInteractText();
            Debug.Log($"Highlight interactText: {interactText}");
        }

        public override void Interact()
        {
            if (isRespawning) return; // Prevent interaction while respawning

            if (price != 0 && CoinManager.Instance.useCoins && CoinManager.Instance.CheckIfEnoughCoins(price) || price == 0)
            {
                StartCoroutine(GetLoot());
            }
        }

        private IEnumerator GetLoot()
        {
            if (price != 0)
            {
                CoinManager.Instance.RemoveCoins(price);
                UIEvents.onCoinsChange?.Invoke(CoinManager.Instance.coins);
            }

            yield return new WaitForSeconds(delayToReceiveLoot);
            GameObject lootObject = loot[Random.Range(0, loot.Length)];

            SpawnSelectedLoot(lootObject);

            anim.Play("LootboxOpen");
            audioSource.Play();

            StartCoroutine(RespawnLootbox());
            isRespawning = true; // Set the flag to true
            SetInteractText();
            Debug.Log("Lootbox collected, interactText set to Wait");
            this.enabled = false; // Disable the interaction component
        }

        private void SpawnSelectedLoot(GameObject loot)
        {
            float spawnAngle = Random.Range(minSpawnAngle, maxSpawnAngle);
            Quaternion spawnRotation = Quaternion.Euler(0f, spawnAngle, 0f); // Rotate around the y-axis
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 spawnDirection = directionDependsOnPlayerPosition ? (player.position - transform.position).normalized * spawnDistance : spawnRotation * -transform.right;

            Vector3 spawnPosition = transform.position + spawnDirection * spawnDistance;

            Instantiate(loot, spawnPosition, spawnRotation);
        }

        private IEnumerator RespawnLootbox()
        {
            float remainingTime = respawnTimes[Mathf.Min(respawnIndex, respawnTimes.Length - 1)];

            while (remainingTime > 0)
            {
                if (countdownText != null)
                {
                    countdownText.text = $"Respawning in: {remainingTime:F1} seconds";
                }

                yield return new WaitForSeconds(1f);
                remainingTime -= 1f;
            }

            if (countdownText != null)
            {
                countdownText.text = string.Empty;
            }

            // Reset the lootbox to its initial state
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            transform.localScale = initialScale;

            anim.Stop();
            anim.Play("LootboxOpen");
            anim["LootboxOpen"].time = 0;
            anim.Sample();
            anim.Stop();

            this.enabled = true; // Re-enable the interaction component
            isRespawning = false; // Reset the flag

            respawnIndex++; // Move to the next respawn time in the array

            SetInteractText();
            Debug.Log("Lootbox ready, interactText set to Collectible");
        }

        private void OnDrawGizmosSelected()
        {
            if (directionDependsOnPlayerPosition) return;

            Gizmos.color = Color.blue;

            Vector3 forward = -transform.right * spawnDistance;

            Quaternion minRotation = Quaternion.Euler(0f, minSpawnAngle, 0f);
            Vector3 minDirection = minRotation * forward;

            Quaternion maxRotation = Quaternion.Euler(0f, maxSpawnAngle, 0f);
            Vector3 maxDirection = maxRotation * forward;

            Gizmos.DrawLine(transform.position, transform.position + minDirection);
            Gizmos.DrawLine(transform.position, transform.position + maxDirection);
        }
    }
}
