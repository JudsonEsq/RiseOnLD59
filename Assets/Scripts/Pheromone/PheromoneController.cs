using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PheromoneController : MonoBehaviour
{
    public InputSystem_Actions controls;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Button FoodButton;

    [SerializeField]
    private Button DangerButton;

    [SerializeField]
    private Button AttackButton;

    [SerializeField]
    private Button GiftButton;

    [SerializeField]
    private GameObject basePheromone;

    private Pheromone.PheromoneType selectedPheromone = Pheromone.PheromoneType.None;

    void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Interact.performed += _ => PlacePheromone();
    }

    void Start()
    {
        FoodButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Food));
        DangerButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Danger));
        AttackButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Attack));
        GiftButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Gift));
    }

    void Update()
    {
        if(Mouse.current.leftButton.IsPressed() && selectedPheromone != Pheromone.PheromoneType.None)
        {
            PlacePheromone();
        }
    }

    public void SelectPheromone(Pheromone.PheromoneType type)
    {
        selectedPheromone = type;
    }

    public void PlacePheromone()
    {
        Vector2 screenPoint = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
        Ray ray = cam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) 
        {
            GameObject GO_Pheromone = Instantiate<GameObject>(basePheromone, hit.point, Quaternion.identity);
            Pheromone pheromone = GO_Pheromone.GetComponent<Pheromone>();
            pheromone.pheromoneType = selectedPheromone;
        }

        selectedPheromone = Pheromone.PheromoneType.None;
    }
}
