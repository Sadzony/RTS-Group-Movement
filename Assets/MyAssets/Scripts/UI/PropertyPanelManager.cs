using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyPanelManager : MonoBehaviour
{
    //Singleton Template
    private static PropertyPanelManager instance;
    public static PropertyPanelManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public HashSet<PropertyPanelSlider> sliders = new HashSet<PropertyPanelSlider>();

    public void ResetToDefaults()
    {
        foreach(PropertyPanelSlider s in sliders)
        {
            s.slider.value = s.defaultValue;
        }
    }
    public void UpdateNeighbourDistance(PropertyPanelSlider s)
    {
        GroupManager.Instance.neighbourDistance = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateUnitSpeed(PropertyPanelSlider s)
    {
        SteeringData.Instance.unitSpeed = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateUnitRotationTime(PropertyPanelSlider s)
    {
        SteeringData.Instance.unitRotationTime = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }

    public void UpdateWaypointNear(PropertyPanelSlider s)
    {
        SteeringData.Instance.waypointNearDistance = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }

    public void UpdateGoalGrowth(PropertyPanelSlider s)
    {
        SteeringData.Instance.goalAreaGrowthMultiplier = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateGoalReachSpeed(PropertyPanelSlider s)
    {
        SteeringData.Instance.goalReachedSpeed = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateSteerPower(PropertyPanelSlider s)
    {
        SteeringData.Instance.steeringPower = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateFlockPower(PropertyPanelSlider s)
    {
        SteeringData.Instance.flockingPower = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateSeekWeight(PropertyPanelSlider s)
    {
        SteeringData.Instance.seekWeight = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateSeparationWeight(PropertyPanelSlider s)
    {
        SteeringData.Instance.separationWeight = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateSeparationDistance(PropertyPanelSlider s)
    {
        SteeringData.Instance.separationDistance = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateCohesionWeight(PropertyPanelSlider s)
    {
        SteeringData.Instance.cohesionWeight = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateCohesionDistance(PropertyPanelSlider s)
    {
        SteeringData.Instance.cohesionDistance = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateAlignmentWeight(PropertyPanelSlider s)
    {
        SteeringData.Instance.alignmentWeight = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }
    public void UpdateAlignmentDistance(PropertyPanelSlider s)
    {
        SteeringData.Instance.alignmentDistance = s.slider.value;
        s.textValue.text = s.slider.value.ToString();
    }

}
