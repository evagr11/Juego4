using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovementTypes : MonoBehaviour
{
    [SerializeField] private MovementStats currentStats;
    [SerializeField] private MovementStats[] availableMovements;
    private int currentMovementIndex = 0;

    [SerializeField] private Image movementImage;
    [SerializeField] private TMP_Text movementText;

    private float displayTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentStats = availableMovements[currentMovementIndex];
        movementImage.gameObject.SetActive(false);
        movementText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeMovementMode();
        }

        displayTime -= Time.deltaTime;
        if (displayTime <= 0)
        {
            movementImage.gameObject.SetActive(false);
            movementText.gameObject.SetActive(false);
        }
    }

    private void ChangeMovementMode()
    {
        currentMovementIndex++;

        if (currentMovementIndex >= availableMovements.Length)
        {
            currentMovementIndex = 0;
        }

        currentStats = availableMovements[currentMovementIndex];

        movementText.text = "Mode: " + currentStats.name;
        movementImage.gameObject.SetActive(true);
        movementText.gameObject.SetActive(true);

        displayTime = 2f;

        FindObjectOfType<Movement>().UpdateStats(currentStats);
    }
}
