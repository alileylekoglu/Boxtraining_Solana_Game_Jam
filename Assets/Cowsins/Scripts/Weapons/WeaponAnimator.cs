using UnityEngine;

namespace cowsins
{
    public class WeaponAnimator : MonoBehaviour
    {
        private PlayerMovement player;
        private WeaponController wc;
        private InteractManager interactManager;
        private Rigidbody rb;

        void Start()
        {
            player = GetComponent<PlayerMovement>();
            wc = GetComponent<WeaponController>();
            interactManager = GetComponent<InteractManager>();
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (wc.inventory[wc.currentWeapon] == null) return;
            if (wc.Reloading || wc.shooting || player.isCrouching || !player.grounded || rb.velocity.magnitude < 0.1f || wc.isAiming
                || wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Unholster")
                || wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("shot"))
            {
                CowsinsUtilities.StopAnim("walking", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());
                CowsinsUtilities.StopAnim("running", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());
                return;
            }

            if (rb.velocity.magnitude > player.crouchSpeed && !wc.shooting && player.currentSpeed < player.runSpeed && player.grounded && !interactManager.inspecting) CowsinsUtilities.PlayAnim("walking", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());
            else CowsinsUtilities.StopAnim("walking", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());

            if (player.currentSpeed >= player.runSpeed && player.grounded) CowsinsUtilities.PlayAnim("running", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());
            else CowsinsUtilities.StopAnim("running", wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>());
        }

        public void StopWalkAndRunMotion()
        {
            if (!wc) return; // Ensure there is a reference for the Weapon Controller before running the following code
            Animator weapon = wc.inventory[wc.currentWeapon].GetComponentInChildren<Animator>();
            CowsinsUtilities.StopAnim("inspect", weapon);
            CowsinsUtilities.StopAnim("walking", weapon);
            CowsinsUtilities.StopAnim("running", weapon);
        }
    }

}