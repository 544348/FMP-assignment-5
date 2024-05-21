using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Interact : MonoBehaviour
{

    public GameObject textbox;
    public Sprite NPCFace;
    private TextMeshProUGUI dialogueText;
    public float textSpeed;
    private int totalIndex;
    private int NPCIndex;
    public int[] playerLineOrder;
    public int[] NPCLineOrder;
    public int playerIndex;
    public string[] NPCLines;
    public string[] playerLines;
    private int totalAmountOfLines;
    public bool playerIsSpeaking;
    public bool isBeingInteractedWith;
    public GameObject eInteract;
    public GameObject CanvasObject;
    public GameObject player;
    private bool hasClicked;
    public bool hasObjectAttached;
    public GameObject wall;

    void Start()
    {
        DisableeInteract();
        dialogueText.text = string.Empty;
        NPCIndex = 0;
        totalIndex = 0; 
        totalAmountOfLines = (NPCLines.Length + playerLines.Length)-1;
        isBeingInteractedWith = false;
    }
    private void Awake()
    {
        dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
    }
    /* private void OnTriggerEnter2D(Collider2D collision)
     {
         Debug.Log("hit");
         ActiveeInteract();
     }

     private void OnTriggerExit2D(Collider2D collision)
     {
         Debug.Log("exit");
         DisableeInteract();
     }
    */

    IEnumerator typeLine()
    {
        GameObject speakerImage = textbox.transform.GetChild(1).gameObject;
        if (!playerIsSpeaking)
        {
            Debug.Log("SpeakerImageIs " + speakerImage.gameObject);
            Debug.Log("NPCisSpeaking");
            speakerImage.GetComponent<Image>().sprite = NPCFace;
            foreach (char letter in NPCLines[NPCIndex].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(textSpeed);

            }
        }
        else 
        {
            Debug.Log("PlayerisSpeaking");
            speakerImage.GetComponent<Image>().sprite = player.GetComponent<PlayerMovement>().playersFace;
            foreach (char letter in playerLines[playerIndex].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(textSpeed);
            }
        }
    }
    public void DisableeInteract()
    {
        dialogueText.text = string.Empty;
        eInteract.SetActive(false);
        textbox.SetActive(false);
        StopAllCoroutines();
        isBeingInteractedWith = false;
        player.GetComponent<PlayerMovement>().playerIsInteracting = false;
    }
    public void ClearDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = string.Empty;
    }
    public void switchToNextLine()
    {
        if (totalIndex < totalAmountOfLines)
        {
            totalIndex++;
         /*   if (totalIndex == NPCLineOrder.Length + playerLineOrder.Length)
            {
                return;
            } */
            if (!playerIsSpeaking)
            {
                NPCIndex++;
            }
            else
            {
                playerIndex++;
            }
            foreach (int playersLines in playerLineOrder)
            {
                Debug.Log("TotalIndex = " + totalIndex);
                if (totalIndex == playersLines)
                {
                    playerIsSpeaking = true;
                    Debug.Log("TotalIndex = " + totalIndex + "playerIndex = " + playerIndex);
                    break;
                }
            }
            foreach (int NPCsLines in NPCLineOrder)
            {
                if (totalIndex == NPCsLines)
                {
                    playerIsSpeaking = false;
                    Debug.Log("TotalIndex = " + totalIndex + "NPCIndex = " + NPCIndex);
                    break;
                }
            }
            dialogueText.text = string.Empty;
            StartCoroutine(typeLine());
            hasClicked = false;
        }
        else
        {
            if (hasObjectAttached)
            {
                Destroy(wall);
                Debug.Log("Destroy wall");
            }
            DisableeInteract();
        }
    }
    public void ActiveeInteract()
    {
        eInteract.SetActive(true);
    }
    public void interactWithNPC()
    {
        textbox.SetActive(true);
        isBeingInteractedWith = true;
        totalIndex = 0;
        ClearDialogue();
        foreach (int playersLines in playerLineOrder)
        {
            if (totalIndex == playersLines)
            {
                playerIsSpeaking = true;
                if (playerIsSpeaking)
                {
                    NPCIndex = 0;
                    playerIndex = 0;
                }
                else
                {
                    NPCIndex = 0;
                    playerIndex = 0;
                }
                StartCoroutine(typeLine());
                return;
            }
            else
            {
                playerIsSpeaking = false;
            }
        }
        if (playerIsSpeaking)
        {
            NPCIndex = 0;
            playerIndex = 0; 
        }
        else 
        {
            NPCIndex = 0;
            playerIndex = 0;
        }
        StartCoroutine(typeLine());
        return;
    }
    private void Update()
    {
        if (isBeingInteractedWith)
        {
            if (Input.GetMouseButtonDown(0)) //maybe change to E
            {
                if (totalIndex == totalAmountOfLines)
                {
                    switchToNextLine();
                    return;
                }
                if (dialogueText.text != string.Empty && dialogueText.text == NPCLines[NPCIndex] || dialogueText.text == playerLines[playerIndex])
                {
                    Debug.Log("NPCindex was less than 0");
                    switchToNextLine();
                }
                else
                {
                    StopAllCoroutines();
                    if (playerIsSpeaking)
                    {
                        dialogueText.text = playerLines[playerIndex];
                    }
                    else
                    {
                        dialogueText.text = NPCLines[NPCIndex];
                    }
                    Debug.Log("wait for NPC to finish speaking");
                }
            }
        }
    }
}
