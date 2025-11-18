using UnityEngine;
public class WallCheck: MonoBehaviour
{
        private GameObject mushroom;

        private void Start()
        {
            mushroom = this.transform.parent.gameObject;
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            // if hit a wall out of cooldown rotate
            if (other.gameObject.CompareTag("Ground") && (Time.time - mushroom.GetComponent<SpikeMushroom>().lastTimeHitAWall) >= 0.2)
            {
                mushroom.GetComponent<SpikeMushroom>().lastTimeHitAWall = Time.time;
                mushroom.GetComponent<SpikeMushroom>().Flip();
            }
        }
}