using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector] public Collider selectionCollider;
    [HideInInspector] public bool isSelected;

    //Checks whether the unit was kept in the previous squad after finishing a command
    [HideInInspector] public bool keptInSquad = false;

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
    public void ReceiveCommand()
    {
        if(keptInSquad)
        {
            CommandManager.Instance.PopCommand(this);
            keptInSquad = false;
        }
#nullable enable
        if(CommandManager.Instance.GetCommand(this, out Command? command))
        {
            SquadManager.Instance.AddUnitToSquad(command, this);
        }
#nullable disable
    }
    public void CompleteCommand()
    {
        Queue<Command> commandQueue = CommandManager.Instance.GetCommandQueue(this);

        //If this is the last command, keep the squad structure and flag to remove on next receive
        if(commandQueue.Count == 1)
        {
            keptInSquad = true;
        }
        //If there are more commands, receive the next one
        else
        {
            keptInSquad = false;
            CommandManager.Instance.PopCommand(this);
            ReceiveCommand();
        }
    }
}
