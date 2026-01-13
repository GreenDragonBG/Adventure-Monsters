using System.Collections;
using UnityEngine;

public class ChompWave : MonoBehaviour
{
    private GameObject[] plants;
    private BossPlants[] plantScripts;

    [Header("Wave Settings")]
    [SerializeField] public float moveSpeed = 1f;
    [SerializeField] public float delayBetweenPlants = 0.1f;
    [SerializeField] private float startingOffset = -0.5f;

    [Header("Ceiling Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wallCheckDistance = 0.15f;

    void Start()
    {
        plantScripts = GetComponentsInChildren<BossPlants>(true);
        plants = new GameObject[plantScripts.Length];

        for (int i = 0; i < plantScripts.Length; i++)
        {
            plants[i] = plantScripts[i].gameObject;

            // Move to hidden position
            plants[i].transform.localPosition =
                new Vector2(plants[i].transform.localPosition.x, startingOffset);

            // Fully disable plant
            plants[i].SetActive(false);
        }
    }

    public void WaveAttack()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        foreach (GameObject plant in plants)
        {
            Vector3 origin =
                transform.TransformPoint(new Vector3(plant.transform.localPosition.x, 0f, 0f));

            RaycastHit2D hit =
                Physics2D.Raycast(origin, Vector2.up, wallCheckDistance, groundLayer);

            if (hit.collider != null)
                yield break;

            StartCoroutine(MovePlant(plant));
            yield return new WaitForSeconds(delayBetweenPlants);
        }
    }

    private IEnumerator MovePlant(GameObject plant)
    {
        Vector3 startPos = plant.transform.localPosition;
        Vector3 upPos = new Vector3(startPos.x, 0f, startPos.z);

        // Activate plant before movement
        plant.SetActive(true);

        // Start attack animation
        BossPlants script = plant.GetComponent<BossPlants>();
        if (script != null)
            StartCoroutine(script.Attack());

        // Move UP
        while (plant.transform.localPosition.y < 0f)
        {
            plant.transform.localPosition =
                Vector3.MoveTowards(plant.transform.localPosition, upPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        // Move DOWN
        while (plant.transform.localPosition.y > startingOffset)
        {
            plant.transform.localPosition =
                Vector3.MoveTowards(plant.transform.localPosition, startPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Fully disable after retreat
        plant.SetActive(false);
    }
}
