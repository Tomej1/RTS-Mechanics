using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    public void AddSelected(GameObject go)
    {
        // Add the selected target into the dictionary
        int id = go.GetInstanceID();

        // Only add the target if it isnt already in the dictionary
        if (!(selectedTable.ContainsKey(id)))
        {
            selectedTable.Add(id, go);
            go.AddComponent<SelectionComponent>();
            Debug.Log("Added " + id +" to slected dict") ;
        }
    }
    public void Deselect(int id)
    {
        // If the target is in the dictionary, simply remove it
        Destroy(selectedTable[id].GetComponent<SelectionComponent>());
        selectedTable.Remove(id);
    }
    public void DeselectAll()
    {
        // Remove all targets in the dictionary
        foreach(KeyValuePair<int, GameObject> pair in selectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<SelectionComponent>());
            }
        }
        selectedTable.Clear();
    }
}
