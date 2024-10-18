using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespaces
using Units.Player;

namespace FDG.InputManager
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler instance;

        private RaycastHit hit;     // What we hit with our ray

        public List<Transform> selectedUnits = new List<Transform>();

        private bool isDragging = false;

        private Vector3 mousePos;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {

        }

        private void OnGUI()
        {
            if (isDragging)
            {
                // Draw the rectangle
                Rect rect = MultiSelect.GetScreenRect(mousePos, Input.mousePosition);
                MultiSelect.DrawScreenRect(rect, new Color(0f, 0f, 0f, 0.25f));
                MultiSelect.DrawScreenRectBorder(rect, 3, Color.blue);
            }
        }

        public void HandleUnitMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Set the mouse position
                mousePos = Input.mousePosition;
                // Create a ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Check if we hit something
                if(Physics.Raycast(ray, out hit))
                {
                    // If we do, then do something with that unit
                    LayerMask layerHit = hit.transform.gameObject.layer;

                    switch (layerHit.value)
                    {
                        case 8: // Units Layer
                            // Do something
                            SelectUnit(hit.transform, Input.GetKey(KeyCode.LeftShift));
                            break;

                        default: // If none of the above happens
                            isDragging = true;
                            DeSelectUnits();
                            break;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                foreach(Transform child in FDG.Player.PlayerManager.instance.playerUnits)
                {
                    foreach(Transform unit in child)
                    {
                        if (IsWithinSelectionBounds(unit))
                        {
                            SelectUnit(unit, true);
                        }
                    }
                }
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1) && HaveSelectedUnits())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Check if we hit something
                if (Physics.Raycast(ray, out hit))
                {
                    // If we do, then do something with that unit
                    LayerMask layerHit = hit.transform.gameObject.layer;

                    switch (layerHit.value)
                    {
                        case 8: // Units Layer
                            // Do something
                            break;

                        case 9: // Enemy units layer
                            // attack or set target
                            break;

                        default: // If none of the above happens
                            // do something
                            foreach(Transform unit in selectedUnits)
                            {
                                PlayerUnit playerUnit = unit.gameObject.GetComponent<PlayerUnit>();
                                playerUnit.MoveUnit(hit.point);
                            }
                            break;
                    }
                }
            }
        }

        private void SelectUnit(Transform unit, bool canMultiSelect = false)
        {
            if (!canMultiSelect)
            {
                DeSelectUnits();
            }
            selectedUnits.Add(unit);
            // Lets set an obj on the unit called Highlight
            unit.Find("Highlight").gameObject.SetActive(true);
        }
        private void DeSelectUnits()
        {
            for(int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].Find("Highlight").gameObject.SetActive(false);
            }
            selectedUnits.Clear();
        }

        private bool IsWithinSelectionBounds(Transform transform)
        {
            if (!isDragging)
            {
                return false;
            }

            Camera cam = Camera.main;
            Bounds vpBounds = MultiSelect.GetVPBounds(cam, mousePos, Input.mousePosition);
            return vpBounds.Contains(cam.WorldToViewportPoint(transform.position));
        }

        private bool HaveSelectedUnits()
        {
            if(selectedUnits.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}