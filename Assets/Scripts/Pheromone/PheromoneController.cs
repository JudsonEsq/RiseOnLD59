using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PheromoneController : MonoBehaviour
{
    public InputSystem_Actions controls;

    public AudioHUD audioHUD;

    [SerializeField]
    private LayerMask PlacementLayers;

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

    private GameObject selectedPheromone = null;

    void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Interact.performed += _ => PlacePheromone();
    }

    void Start()
    {
        FoodButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Good));
        DangerButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Hazard));
        AttackButton.onClick.AddListener(() => SelectPheromone(Pheromone.PheromoneType.Attack));
    }

    void Update()
    {
        if (selectedPheromone != null)
        {
            Vector2 screenPoint = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
            Ray ray = cam.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, PlacementLayers)) 
            {
                selectedPheromone.transform.position = hit.point;
            }

            if (Mouse.current.leftButton.IsPressed() && selectedPheromone != null)
            {
                PlacePheromone();
            }

        }
    }

    public void SelectPheromone(Pheromone.PheromoneType type)
    {
        if (selectedPheromone != null)
        {
            Destroy(selectedPheromone);
        }

        Vector2 screenPoint = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
        Ray ray = cam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, PlacementLayers))
        {
            selectedPheromone = Instantiate<GameObject>(basePheromone, hit.point, Quaternion.identity);
            selectedPheromone.GetComponent<Pheromone>().onCursor = true;
            selectedPheromone.GetComponent<Pheromone>().pheromoneType = type;
        }
    }

    public void PlacePheromone()
    {
        Vector2 screenPoint = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
        if (selectedPheromone){
            
            Pheromone.PheromoneType type = selectedPheromone.GetComponent<Pheromone>().pheromoneType;
            audioHUD.PlayPheromonePlacement(type);

            if (PointerIsUIHit(screenPoint))
            {
                Debug.Log("Destroying previously selected pheromone");
                Destroy(selectedPheromone);
            }
            selectedPheromone.GetComponent<Pheromone>().onCursor = false;
            selectedPheromone = null;
        } 
    }

    private bool PointerIsUIHit(Vector2 position)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // UI Elements must have `picking mode` set to `position` to be hit
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.distance == 0 && result.isValid)
                {
                    Debug.Log("Click is UI Hit");
                    return true;
                }
            }
        }

        return false;
    }
}
