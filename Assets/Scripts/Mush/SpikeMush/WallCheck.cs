using UnityEngine;
public class WallCheck: MonoBehaviour
{
        private GameObject mush;

        private void Start()
        {
            mush = this.transform.parent.gameObject;
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            // if hit a wall out of cooldown rotate
            if (other.gameObject.CompareTag("Ground") && (Time.time - mush.GetComponent<SpikeMush>().lastTimeHitAWall) >= 0.2)
            {
                mush.GetComponent<SpikeMush>().lastTimeHitAWall = Time.time;
                mush.GetComponent<SpikeMush>().Flip();
            }
        }
}