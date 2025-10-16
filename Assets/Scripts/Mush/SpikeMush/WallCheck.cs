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
            if (other.gameObject.CompareTag("Ground") && (Time.time - mushroom.GetComponent<MushroomController>().lastTimeHitAWall) >= 0.2)
            {
                mushroom.GetComponent<MushroomController>().lastTimeHitAWall = Time.time;
                mushroom.GetComponent<MushroomController>().Flip();
            }
        }
}