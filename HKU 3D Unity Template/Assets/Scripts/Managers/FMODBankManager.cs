using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class FMODBankManager : MonoBehaviour
{
    public static FMODBankManager Instance { get; private set; }

    [Header("Player + UI Banks Are Always loaded")]
    [SerializeField] private List<string> banksToLoad;

    private HashSet<string> loadedBanks = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitFMOD();
    }

    private void InitFMOD()
    {
        EnsureBankLoaded("UIBank");
        EnsureBankLoaded("Master");
        EnsureBankLoaded("Master.strings");

        foreach (var bank in banksToLoad)
        {
            EnsureBankLoaded(bank);
        }

        AudioManagerScript.Instance.ConnectBusVCA();
    }


    public void EnsureBankLoaded(string bankName)
    {
        if (string.IsNullOrEmpty(bankName))
        {
            //No string here
            return;
        }

        if (loadedBanks.Contains(bankName))
        {
            //Already Loaded (YIPPIE)
            return;
        }

        RuntimeManager.LoadBank(bankName, true);
        loadedBanks.Add(bankName);
    }

    public void UnloadBank(string bankName)
    {
        if (!loadedBanks.Contains(bankName))
        {
            //Bank not loaded
            return;
        }

        RuntimeManager.UnloadBank(bankName);
        loadedBanks.Remove(bankName);
    }
}
