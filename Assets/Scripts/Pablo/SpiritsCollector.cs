using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritsCollector : MonoBehaviour
{
    public List<GameObject> jaguar = new List<GameObject>();
    public List<GameObject> tucan = new List<GameObject>();
    public List<GameObject> tapir = new List<GameObject>();
    public List<GameObject> mono = new List<GameObject>();
    public Text jaguarText;
    public Text tucanText;
    public Text tapirText;
    public Text monoText;
    bool win;

    public static SpiritsCollector instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddSpirit(SpiritType type, GameObject spirit)
    {
        switch (type)
        {
            case SpiritType.Jaguar:
                jaguar.Add(spirit);
                jaguarText.text = $"{jaguar.Count}/5";
                break;
            case SpiritType.Tucan:
                tucan.Add(spirit);
                tucanText.text = $"{tucan.Count}/5";
                break;
            case SpiritType.Tapir:
                tapir.Add(spirit);
                tapirText.text = $"{tapir.Count}/5";
                break;
            case SpiritType.Mono:
                mono.Add(spirit);
                monoText.text = $"{mono.Count}/5";
                break;
        }

        CheckWinCondition();
        spirit.SetActive(false);
        spirit.transform.parent = this.transform;
        spirit.transform.localPosition = new Vector3(0, 0, -1);
    }

    private void RemoveSpirit(SpiritType type)
    {
        if (win) return;
        GameObject spiritToRemove = null;

        switch (type)
        {
            case SpiritType.Jaguar:
                spiritToRemove = jaguar[0];
                jaguar.RemoveAt(0);
                jaguarText.text = $"{jaguar.Count}/5";
                break;
            case SpiritType.Tucan:
                spiritToRemove = tucan[0];
                tucan.RemoveAt(0);
                tucanText.text = $"{tucan.Count}/5";
                break;
            case SpiritType.Tapir:
                spiritToRemove = tapir[0];
                tapir.RemoveAt(0);
                tapirText.text = $"{tapir.Count}/5";
                break;
            case SpiritType.Mono:
                spiritToRemove = mono[0];
                mono.RemoveAt(0);
                monoText.text = $"{mono.Count}/5";
                break;
        }

        spiritToRemove.transform.parent = null;
        spiritToRemove.SetActive(true);
    }

    public void ReleaseRandomSpirit(int count = 1)
    {
        Debug.Log("Releasing a random spirit...");
        int totalSpirits = jaguar.Count + tucan.Count + tapir.Count + mono.Count;
        if (totalSpirits == 0) return;

        int _count = 0;
        bool exit = false;
        while (!exit)
        {
            int randomType = Random.Range(0, 4);
            switch (randomType)
            {
                case 0:
                    if (jaguar.Count > 0)
                    {
                        RemoveSpirit(SpiritType.Jaguar);
                        _count++;
                        if (_count >= count)
                            exit = true;
                    }
                    break;
                case 1:
                    if (tucan.Count > 0)
                    {
                        RemoveSpirit(SpiritType.Tucan);
                        _count++;
                        if (_count >= count)
                            exit = true;
                    }
                    break;
                case 2:
                    if (tapir.Count > 0)
                    {
                        RemoveSpirit(SpiritType.Tapir);
                        _count++;
                        if (_count >= count)
                            exit = true;
                    }
                    break;
                case 3:
                    if (mono.Count > 0)
                    {
                        RemoveSpirit(SpiritType.Mono);
                        _count++;
                        if (_count >= count)
                            exit = true;
                    }
                    break;
            }
        }
    }

    private void CheckWinCondition()
    {
        if (jaguar.Count >= 5 && tucan.Count >= 5 && tapir.Count >= 5 && mono.Count >= 5)
        {
            win = true;
            Debug.Log("You Win!");
            // Aquí puedes agregar lógica adicional para manejar la victoria
        }
    }
}

public enum SpiritType
{
    Jaguar = 0,
    Tucan = 1,
    Tapir = 2,
    Mono = 3
}