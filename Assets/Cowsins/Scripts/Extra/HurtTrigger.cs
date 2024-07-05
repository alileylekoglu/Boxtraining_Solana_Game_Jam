using UnityEngine;
using System.Collections;

namespace cowsins
{
    public class HurtTrigger : MonoBehaviour
    {
        [SerializeField] private float damage;
        private Coroutine damageCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(ApplyDamage(other.GetComponent<PlayerStats>()));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }

        private IEnumerator ApplyDamage(PlayerStats playerStats)
        {
            while (true)
            {
                playerStats.Damage(damage);
                yield return new WaitForSeconds(2f);
            }
        }
    }
}
