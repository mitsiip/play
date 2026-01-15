using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public enum MissionStep { TakeCargo, DeliverCargo, ScanObject, CollectEnergy, Complete }

    public MissionStep currentStep = MissionStep.TakeCargo;
    public PlayerDrone player;
    public GameObject cargoPrefab;

    public Transform stationAPoint;
    public Transform deliveryPoint;
    public Transform scanTarget;
    public GameObject energySpheresGroup;

    public TextMeshProUGUI missionStatusText;
    public TextMeshProUGUI distanceHUD;
    public Slider healthSlider;

    private float scanProgress = 0f;

    void Start()
    {
        if (energySpheresGroup != null) energySpheresGroup.SetActive(false);
    }

    void Update()
    {
        UpdateUI();
        CheckMissionProgress();
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = player.health;

        string taskText = "";
        float distance = 0f;

        switch (currentStep)
        {
            case MissionStep.TakeCargo:
                taskText = "заберите груз со станции 1 (кнопка E)";
                distance = Vector3.Distance(player.transform.position, stationAPoint.position);
                break;

            case MissionStep.DeliverCargo:
                taskText = "доставьте груз на станцию 2 (кнопка E)";
                distance = Vector3.Distance(player.transform.position, deliveryPoint.position);
                break;

            case MissionStep.ScanObject:
                float progressPercent = Mathf.Clamp((scanProgress / 5f) * 100f, 0, 100);
                taskText = $"сканирование астероида: {Mathf.Round(progressPercent)}%";

                distance = Vector3.Distance(player.transform.position, scanTarget.position);
                if (distance > 8f) taskText += "\nподлетите близко";
                break;

            case MissionStep.CollectEnergy:
                taskText = "соберите все цели";
                int spheresLeft = energySpheresGroup.transform.childCount;
                taskText += $"\nосталось {spheresLeft}";
                if (spheresLeft == 0) currentStep = MissionStep.Complete;
                break;

            case MissionStep.Complete:
                taskText = "победа";
                break;
        }

        missionStatusText.text = taskText;

        if (currentStep != MissionStep.Complete && currentStep != MissionStep.CollectEnergy)
            distanceHUD.text = $"расстояние: {Mathf.Round(distance)}м";
        else
            distanceHUD.text = "";
    }

    void CheckMissionProgress()
    {
        if (currentStep == MissionStep.TakeCargo && Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(player.transform.position, stationAPoint.position) < 6f)
            {
                PickUpCargoLogic();
                currentStep = MissionStep.DeliverCargo;
            }
        }

        else if (currentStep == MissionStep.DeliverCargo && Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(player.transform.position, deliveryPoint.position) < 6f)
            {
                Destroy(player.currentCargo);
                player.currentCargo = null;
                currentStep = MissionStep.ScanObject;
            }
        }

        else if (currentStep == MissionStep.ScanObject)
        {
            if (Vector3.Distance(player.transform.position, scanTarget.position) < 8f)
            {
                scanProgress += Time.deltaTime;
                if (scanProgress >= 5f)
                {
                    currentStep = MissionStep.CollectEnergy;
                    if (energySpheresGroup != null) energySpheresGroup.SetActive(true);
                }
            }
        }
    }

    void PickUpCargoLogic()
    {
        player.currentCargo = Instantiate(cargoPrefab, player.cargoHold);
        Rigidbody cargoRb = player.currentCargo.GetComponent<Rigidbody>();
        if (cargoRb != null) cargoRb.isKinematic = true;
        player.currentCargo.transform.localPosition = Vector3.zero;
        player.currentCargo.transform.localRotation = Quaternion.identity;
    }

    public void RunSpeedTest()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb.velocity.magnitude <= player.maxVelocity + 0.1f)
            Debug.Log("test success");
        else
            Debug.Log("test fail");
    }
}