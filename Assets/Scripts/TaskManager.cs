using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    int streamerHP = 100;
    public Image Streamer, StreamHP;
    public Sprite streamerIdle, streamerDamage;

    int TimeMod = 0;
    public Slider overheatSlider;
    public Slider hpSlider;
    public Gradient hp;
    public Gradient time;
    Color overheatColor;
    public Text TaskName;

    int cpuCount, ramCount, gpuCount;
    int cpuLvl = 1, ramLvl = 1, gpuLvl = 1;
    public TMP_Text cpu, ram, gpu;
    public TMP_Text cpuLV, ramLV, gpuLV;

    int money = 0;
    public TMP_Text moneyText;

    public GameObject MainPanel, GameOver, WinScreen;

    List<Task> Tasks = new List<Task>();

    public GameObject Task1, Task2, Task3, Task4;

    public class Task
    {
        public string name;
        public int maxTime;
        public int diff;
        // instead of setting up which resource does more damage it will be calculated based on the name
        public int hp;
        public Dictionary<string, int> effectiveness = new Dictionary<string, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // start by giving out five random tasks, some resources, and setting up timer-based events

        StreamHP.color = new Color32(114, 255, 0, 255);

        for (int i = 0; i < 5; i++)
            AddTask(Random.Range(0, 3));

        cpuCount = Random.Range(7, 15);
        gpuCount = Random.Range(10, 15);
        ramCount = Random.Range(5, 10);

        cpu.text = cpuCount.ToString();
        gpu.text = gpuCount.ToString();
        ram.text = ramCount.ToString();

        cpuLV.text = cpuLvl.ToString();
        gpuLV.text = gpuLvl.ToString();
        ramLV.text = ramLvl.ToString();

        StartCoroutine(TaskTimer());
        StartCoroutine(ResourceRefresher());
        StartCoroutine(TimeModIncrease());
        StartCoroutine(WinScreenTimer());

        overheatColor = overheatSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;

        ChangeTask();
    }

    // Update is called once per frame
    void Update()
    {
        // each frame update the urgency slider with Tasks[0]'s time

        moneyText.text = money.ToString();

        overheatSlider.value += Time.deltaTime;
        overheatColor = time.Evaluate(overheatSlider.value / overheatSlider.maxValue);
        overheatSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = overheatColor;

        if (overheatSlider.value >= overheatSlider.maxValue)
        {
            streamerHP -= Tasks[0].diff * 5;
            if (streamerHP > 0)
            {
                StreamHP.color = hp.Evaluate(streamerHP / 100f);
                Streamer.sprite = streamerDamage;
                //play an "oof" sound effect
                StartCoroutine(ChangeSprite());
                Tasks.RemoveAt(0);
                ChangeTask();
            }
            else
            {
                StopAllCoroutines();
                GameOver.SetActive(true);
                MainPanel.SetActive(false);
            }
        }

        if (Tasks.Count >= 2)
            Task2.SetActive(true);
        else
            Task2.SetActive(false);
        if (Tasks.Count >= 3)
            Task3.SetActive(true);
        else
            Task3.SetActive(false);
        if (Tasks.Count >= 4)
            Task4.SetActive(true);
        else
            Task4.SetActive(false);
    }

    void ChangeTask()
    {
        if (Tasks.Count > 0)
        {
            Task1.SetActive(true);
            hpSlider.gameObject.SetActive(true);
            overheatSlider.gameObject.SetActive(true);

            hpSlider.maxValue = Tasks[0].hp;
            hpSlider.value = Tasks[0].hp;
            hpSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(114, 255, 0, 255);

            overheatSlider.maxValue = Tasks[0].maxTime;
            overheatSlider.value = 0;

            TaskName.text = Tasks[0].name;
        }
        else
        {
            Task1.SetActive(false);
            hpSlider.gameObject.SetActive(false);
            overheatSlider.gameObject.SetActive(false);
        }
    }

    void AddTask(int type)
    {
        Task newTask = new Task();
        switch (type)
        {
            case 0:
                newTask.name = "Взрыв";
                newTask.maxTime = 5;
                newTask.diff = 3;
                newTask.hp = 25 + (25 * TimeMod);
                newTask.effectiveness.Add("CPU", 3);
                newTask.effectiveness.Add("GPU", 4);
                newTask.effectiveness.Add("RAM", 1);
                break;
            case 1:
                newTask.name = "Движение врагов";
                newTask.maxTime = 10;
                newTask.diff = 2;
                newTask.hp = 15 + (15 * TimeMod);
                newTask.effectiveness.Add("CPU", 4);
                newTask.effectiveness.Add("GPU", 2);
                newTask.effectiveness.Add("RAM", 3);
                break;
            case 2:
                newTask.name = "Движение игрока";
                newTask.maxTime = 10;
                newTask.diff = 1;
                newTask.hp = 10 + (10 * TimeMod);
                newTask.effectiveness.Add("CPU", 4);
                newTask.effectiveness.Add("GPU", 1);
                newTask.effectiveness.Add("RAM", 2);
                break;
            case 3:
                newTask.name = "Браузер";
                newTask.maxTime = 15;
                newTask.diff = 2;
                newTask.hp = 10 + (10 * TimeMod);
                newTask.effectiveness.Add("CPU", 1);
                newTask.effectiveness.Add("GPU", 1);
                newTask.effectiveness.Add("RAM", 3);
                break;
        }
        Tasks.Add(newTask);
        if (Tasks.Count == 1)
            ChangeTask();
    }

    public void Spend(string resource)
    {
        bool canAct = false;

        if (Tasks.Count > 0)
        {
            switch (resource)
            {
                case "CPU":
                    if (cpuCount > 0)
                    {
                        cpuCount--;
                        cpu.text = cpuCount.ToString();
                        canAct = true;
                    }
                    break;
                case "GPU":
                    if (gpuCount > 0)
                    {
                        gpuCount--;
                        gpu.text = gpuCount.ToString();
                        canAct = true;
                    }
                    break;
                case "RAM":
                    if (ramCount > 0)
                    {
                        ramCount--;
                        ram.text = ramCount.ToString();
                        canAct = true;
                    }
                    break;
            }

            if (canAct)
            {
                hpSlider.value -= 1 * Tasks[0].effectiveness[resource];

                hpSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = hp.Evaluate(hpSlider.value / 100f);
                if (hpSlider.value <= 0)
                {
                    money += Tasks[0].diff * 35;
                    moneyText.text = money.ToString();
                    Tasks.RemoveAt(0);
                    ChangeTask();
                }
            }
        }
    }

    public void Exchange(string resource)
    {
        switch (resource)
        {
            case "CPU":
                if (cpuCount >= gpuLvl * 2)
                {
                    cpuCount -= gpuLvl * 2;
                    gpuCount++;
                    cpu.text = cpuCount.ToString();
                    gpu.text = gpuCount.ToString();
                }
                else
                {
                    //play a sound effect
                }
                break;
            case "GPU1":
                if (gpuCount >= cpuLvl * 2)
                {
                    gpuCount -= cpuLvl * 2;
                    cpuCount++;
                    cpu.text = cpuCount.ToString();
                    gpu.text = gpuCount.ToString();
                }
                break;
            case "GPU2":
                if (gpuCount >= ramLvl * 2)
                {
                    gpuCount -= ramLvl * 2;
                    ramCount++;
                    gpu.text = gpuCount.ToString();
                    ram.text = ramCount.ToString();
                }
                break;
            case "RAM":
                if (ramCount >= gpuLvl * 2)
                {
                    ramCount -= gpuLvl * 2;
                    gpuCount++;
                    gpu.text = gpuCount.ToString();
                    ram.text = ramCount.ToString();
                }
                break;
        }
    }

    public void Upgrade(string resource)
    {
        switch (resource)
        {
            case "CPU":
                if (money >= 50 * cpuLvl)
                {
                    money -= 50 * cpuLvl;
                    moneyText.text = money.ToString();
                    cpuLvl++;
                    cpuLV.text = cpuLvl.ToString();
                }
                break;
            case "GPU":
                if (money >= 50 * gpuLvl)
                {
                    money -= 50 * gpuLvl;
                    moneyText.text = money.ToString();
                    gpuLvl++;
                    gpuLV.text = gpuLvl.ToString();
                }
                break;
            case "RAM":
                if (money >= 50 * ramLvl)
                {
                    money -= 50 * ramLvl;
                    moneyText.text = money.ToString();
                    ramLvl++;
                    ramLV.text = ramLvl.ToString();
                }
                break;
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator TaskTimer()
    {
        while (true)
        {
            //select a random taskType and pass it to AddTask
            AddTask(Random.Range(0, 3));
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator ResourceRefresher()
    {
        while (true)
        {
            cpuCount += cpuLvl;
            ramCount += ramLvl;
            gpuCount += gpuLvl;

            cpu.text = cpuCount.ToString();
            gpu.text = gpuCount.ToString();
            ram.text = ramCount.ToString();
    
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator TimeModIncrease()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            TimeMod++;
        }
    }

    IEnumerator ChangeSprite()
    {
        yield return new WaitForSeconds(1);
        Streamer.sprite = streamerIdle;
    }

    IEnumerator WinScreenTimer()
    {
        yield return new WaitForSeconds(300);
        StopCoroutine(TaskTimer());
        StopCoroutine(TimeModIncrease());
        while (true)
        {
            if (Tasks.Count <= 0)
            {
                WinScreen.SetActive(true);
                MainPanel.SetActive(false);
                break;
            }
            else
                yield return null;
        }
    }
}
