using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector] public Collider selectionCollider;
    [HideInInspector] public bool isSelected;
    private GameObject selectionSprite;

    // Start is called before the first frame update
    void Start()
    {
        SelectionManager.Instance.selectableUnits.Add(this);
        selectionSprite = this.gameObject.transform.Find("SelectionSprite").gameObject;
        selectionCollider = this.gameObject.transform.Find("SelectionCollider").GetComponent<Collider>();
    }
    public void Die()
    {
        Transform mesh = transform.Find("HPCharacter");
        mesh.GetComponent<AgentAnimationController>().enabled = false;
        mesh.GetComponent<Animator>().SetTrigger("death");

        SelectionManager.Instance.selectableUnits.Remove(this);
        SelectionManager.Instance.Deselect(this);

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
}
