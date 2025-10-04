using UnityEngine;

public class Interact_artifact : MonoBehaviour
{
    public GameObject[] artefacts;
    public GameObject[] artefact_Panels;
    public GameObject[] Interact_UI; 
    void Start()
    {
        gameObject.SetActive(true);

        for (int x = 0; x < artefacts.Length; x++) {
            Interact_UI[x].SetActive(false);
        }

        
    }

    public void OnTriggerEnter2D (Collider2D other)
    {
        for (int i = 0; i < artefacts.Length; i++)
        {
            if (other.gameObject ==  artefacts[i])
            {
                Interact_UI[i].SetActive(true);
                Debug.Log(artefacts[i].name);
                
                
                
            } 
            
        }
    }
    
    public void OnTriggerExit2D (Collider2D other)
    {
        for (int i = 0; i < artefacts.Length; i++)
        {
            if (other.gameObject ==  artefacts[i])
            {
                Interact_UI[i].SetActive(false);
                Debug.Log("Exit");
                
                
                
            } 
            
        }
    }


}
