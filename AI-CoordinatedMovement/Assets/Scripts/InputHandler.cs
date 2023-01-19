using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Units.Player;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

namespace RTS.InputManager
{ 
    public class InputHandler : MonoBehaviour
    {
        // Only one duplicate
        public static InputHandler instance;

        [SerializeField] private int maxUnits = 15;

        private RaycastHit raycastHit;
        private const int obstacleLayerValue = 6;
        private const int unitLayerValue = 7;

        private List<Transform> selectedUnits = new List<Transform>();
        private List<Vector3> listOfVectors = new List<Vector3>();
        private float currentAngle = 0;
        private Vector3 shortestDestination;

        private bool isDragging = false;
        private Vector3 mousePos;

        // FSM
        private enum Movement
        {
            noMovement,
            calculatingMovement,
            moveToTarget
        }
        private Movement movement = Movement.noMovement;

        private enum Formation
        {
            rectangle,
            circle
        }
        private Formation formation = Formation.rectangle;

        void Start()
        {
            instance = this;
        }

        private void Update()
        {
            FsmUnitMovement();
        }

        // Draw
        private void OnGUI()
        {
            if (isDragging)
            {
                // Rectangle from where we started (mousePos) and where we currently are (Input.mousePosition)
                Rect rect = DragSelect.GetScreenRect(mousePos, Input.mousePosition);

                DragSelect.DrawSelectionRect(rect, new Color(0f, 0f, 1f, 0.2f));
                DragSelect.DrawSelectionRectBorder(rect, 2, Color.blue);
            }
        }

        private void FsmUnitMovement()
        {
            switch (movement)
            {
                case Movement.noMovement:
                    break;
                case Movement.calculatingMovement:
                    {
                        switch (formation)
                        {
                            case Formation.rectangle:
                                CalculateRectMovement(raycastHit.point);
                                break;
                            case Formation.circle:
                                CalculateCircleMovement(raycastHit.point);
                                break;
                        }
                    }
                    break;
                case Movement.moveToTarget:
                    MoveToTarget();
                    break;
                default:
                    break;
            }
        }

        public void HandleInput()
        {
            // 0 => Left click
            if(Input.GetMouseButtonDown(0))
            {
                mousePos = Input.mousePosition;

                // Create a ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Check if we hit something
                if(Physics.Raycast(ray, out raycastHit))
                { 
                    LayerMask layerHit = raycastHit.transform.gameObject.layer;

                    switch (layerHit.value)
                    {
                        case unitLayerValue:
                            {
                                SelectUnits(raycastHit.transform, Input.GetKey(KeyCode.LeftShift));
                                break;
                            }
                        default:
                            {
                                // Can only draw if our dragging doesn't happen on a unit
                                isDragging = true;
                                DeselectUnits();
                                break;
                            }
                    }
                }
            }

            // Stop the dragging
            if( Input.GetMouseButtonUp(0))
            {   
                // For every child in the PlayerUnits gameobject
                foreach(Transform child in Player.PlayerManager.instance.units)
                {
                    if(IsInSelectionBounds(child))
                        SelectUnits(child, true);
                }

                isDragging = false;
            }

            // Right click
            if(Input.GetMouseButtonUp(1) && HaveSelectedUnits())
            {
                // Create a ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Check if we hit something
                if (Physics.Raycast(ray, out raycastHit))
                {
                    LayerMask layerHit = raycastHit.transform.gameObject.layer;

                    switch (layerHit.value)
                    {
                        case unitLayerValue: // Probably enemy layer
                            {
                                // Move units
                                break;
                            }
                        case obstacleLayerValue:
                            {
                                // Can't move
                                break;
                            }
                        default:
                            {
                                movement = Movement.calculatingMovement;
                                break;
                            }
                    }
                }
            }

            // R - Rectangle
            if (Input.GetKeyUp(KeyCode.R))
            {
                formation = Formation.rectangle;
                Debug.Log(formation.ToString());
            }
            // C - Circle
            if (Input.GetKeyUp(KeyCode.C))
            {
                formation = Formation.circle;
                Debug.Log(formation.ToString());
            }
        }

        private void SelectUnits(Transform unit, bool multipleSelection = false)
        {
            // Deselect the units
            if(!multipleSelection)
                DeselectUnits();

            if (selectedUnits.Count < maxUnits)
            {
                selectedUnits.Add(unit);
                unit.Find("Highlight").gameObject.SetActive(true);
            }

        }
        private bool IsInSelectionBounds(Transform transform)
        {
            if (!isDragging)
                return false;

            Camera camera = Camera.main;
            Bounds viewPortBounds = DragSelect.GetViewPortBounds(camera, mousePos, Input.mousePosition);

            // Can't just do transform.position since that is in 3D space and view port is in 2D
            return viewPortBounds.Contains(camera.WorldToViewportPoint(transform.position));
        }
        private bool HaveSelectedUnits()
        {
            if (selectedUnits.Count > 0)
                return true;

            return false;
        }
        private void DeselectUnits()
        {
            for (int i = 0; i < selectedUnits.Count; ++i)
            {
                selectedUnits[i].Find("Highlight").gameObject.SetActive(false);
            }
            selectedUnits.Clear();
        }

        private void CalculateRectMovement(Vector3 startPosition)
        {
            listOfVectors.Add(startPosition);

            // (directionX, directionZ) is a vector - direction in which we move right now
            float directionX = 2;
            float directionZ = 0;
            // length of current segment
            int segmentLength = 1;

            // current position (x, z) and how much of current segment we passed
            float x = startPosition.x;
            float z = startPosition.z;
            int segmentPassed = 0;
            for (int k = 1; k < selectedUnits.Count; ++k)
            {
                // make a step, add 'direction' vector (directionX, directionZ) to current position (x, z)
                x += directionX;
                z += directionZ;
                ++segmentPassed;

                if (segmentPassed == segmentLength)
                {
                    // done with current segment
                    segmentPassed = 0;

                    // 'rotate' directions
                    float buffer = directionX;
                    directionX = -directionZ;
                    directionZ = buffer;

                    // increase segment length if necessary
                    if (directionZ == 0)
                    {
                        ++segmentLength;
                    }
                }

                Vector3 temp;
                temp.x = x;
                temp.z = z;
                temp.y = .1f;

                listOfVectors.Add(temp);
            }

            movement = Movement.moveToTarget;
        }
        private void CalculateCircleMovement(Vector3 startPosition)
        {
            float radius = selectedUnits.Count / 2;
            float steps = (Mathf.PI * 2) / selectedUnits.Count;

            for(int i = 0; i < selectedUnits.Count; i++)
            {
                Vector3 temporary;
                temporary.x = startPosition.x + Mathf.Sin(currentAngle) * radius;
                temporary.z = startPosition.z + Mathf.Cos(currentAngle) * radius;
                temporary.y = .1f;

                listOfVectors.Add(temporary);

                currentAngle += steps;
            }

            currentAngle = 0;
            movement = Movement.moveToTarget;
        }
        private void MoveToTarget()
        {
            int incr = 0;
            foreach (Transform unit in selectedUnits)
            {
                // Get HandlePlayerUnit component of every unit in our selected units
                HandlePlayerUnit playerUnit = unit.gameObject.GetComponent<HandlePlayerUnit>();
                // Move to this point

                //// SHORTEST DESTINATION
                //for (int i = 0; i < listOfVectors.Count; ++i)
                //{
                //    if (i == 0)
                //    {
                //        // Set the shortest distance to the first position
                //        shortestDestination = listOfVectors[i];
                //    }
                //    else
                //    {
                //        // If the distance is shorter than the distance to the shortest destination
                //        if (Vector3.Distance(unit.position, listOfVectors[i]) < Vector3.Distance(unit.position, shortestDestination))
                //        {
                //            // Set new shortest destination en continue loop
                //            shortestDestination = listOfVectors[i];
                //            continue;
                //        }

                //        continue;
                //    }
                //}
                //playerUnit.SetDestination(shortestDestination);
                //listOfVectors.Remove(shortestDestination);

                playerUnit.SetDestination(listOfVectors[incr]);
                ++incr;
            }

            listOfVectors.Clear();
            movement = Movement.noMovement;
        }

    }
}


