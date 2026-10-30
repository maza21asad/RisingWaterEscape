using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [SerializeField] private Transform cat;                // the Player root (its pivot is at the feet)
    [SerializeField] private float startY = -3f;           // water surface height at the start
    [SerializeField] private float riseSpeed = 0.35f;      // units per second
    [SerializeField] private float startDelay = 2f;        // seconds before it starts rising

    private float surfaceY;
    private float timer;

    private void Awake()
    {
        SetSurface(startY);
    }

    private void OnEnable() { GameManager.LevelReset += ResetWater; }
    private void OnDisable() { GameManager.LevelReset -= ResetWater; }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Playing) return;   // frozen in Menu, Won, Failed

        timer += Time.deltaTime;
        if (timer > startDelay)
            SetSurface(surfaceY + riseSpeed * Time.deltaTime);

        if (cat.position.y < surfaceY)                             // water reached the cat's feet
            gm.Fail();
    }

    private void ResetWater()
    {
        timer = 0f;
        SetSurface(startY);
    }

    // Moves the cube so its TOP face sits at height y
    private void SetSurface(float y)
    {
        surfaceY = y;
        Vector3 p = transform.position;
        p.y = y - transform.localScale.y / 2f;
        transform.position = p;
    }
}