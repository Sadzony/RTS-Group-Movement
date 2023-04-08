using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector] public Collider selectionCollider;
    [HideInInspector] public bool isSelected;

    [HideInInspector] public bool standby = true;

    public float neighbourLoSArc;

    private GameObject selectionSprite;

    // Start is called before the first frame update
    void Start()
    {
        SelectionManager.Instance.AddToSelectable(this);
        selectionSprite = this.gameObject.transform.Find("SelectionSprite").gameObject;
        selectionCollider = this.gameObject.transform.Find("SelectionCollider").GetComponent<Collider>();
    }
    public void Die()
    {
        Transform mesh = transform.Find("HPCharacter");
        mesh.GetComponent<AgentAnimationController>().enabled = false;
        mesh.GetComponent<Animator>().SetTrigger("death");

        SelectionManager.Instance.Deselect(this);
        SelectionManager.Instance.RemoveFromSelectable(this);


        GetComponent<NavMeshAgent>().enabled = false;

        StartCoroutine(DeleteAfterDeath());
    }
    IEnumerator DeleteAfterDeath()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }    

    public void OnSelected()
    {
        isSelected = true;
        selectionSprite.SetActive(true);
    }
    public void OnDeselected()
    {
        isSelected = false;
        selectionSprite.SetActive(false);
    }
    public void OnCommandCancel()
    {
        standby = true;
    }
    public void ReceiveCommand()
    {
#nullable enable
        //try getting the next command. If successful, receive it. If the unit is not in standby, it's ignored
        if (standby == true)
        {
            CommandManager.Instance.OnCommandReceived(this);
            if (CommandManager.Instance.GetCurrentCommand(this, out Command? command))
            {
                standby = false;
            }
        }
#nullable disable
    }
    public void CompleteCommand()
    {
        //Complete the command
        CommandManager.Instance.OnCommandCompleted(this);
        //Since the command is completed, the unit is waiting for the next one, so enter standby mode.
        standby = true;

        //If there are more commands, receive the next one
#nullable enable
        if (CommandManager.Instance.NextCommandAvailable(this))
        {
            ReceiveCommand();
        }
#nullable disable
    }
}
