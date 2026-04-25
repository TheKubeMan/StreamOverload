using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    int hp = 100;
    int overheat = 0;
    public GameObject overheatSlider;
    // public GameObject hpSlider;
    float maxTime = 0;
    public float resourceRefreshRate = 3;
    public int cpuCount, ramCount, gpuCount;
    public Text cpu, ram, gpu;
    List<Task> Tasks = new List<Task>();

    public class Task
    {
        public float maxTime;
        public float diff;
        // there's gotta be a better way to declare which type of resources will be used, but i can't think of that yet
        public string resources;
    }

    // Start is called before the first frame update
    void Start()
    {
        // start by giving out five random tasks, some resources, and setting up timer-based events

        for (int i = 0; i < 5; i++)
            AddTask(Random.Range(0, 3));

        cpuCount = Random.Range(20, 40);
        gpuCount = Random.Range(15, 30);
        ramCount = Random.Range(10, 30);

        StartCoroutine("TaskTimer");
        StartCoroutine("ResourceRefresher");
    }

    // Update is called once per frame
    void Update()
    {
        // each frame update the urgency slider with Tasks[0]'s time and the resources' text

        Color a = overheatSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
        float time = 0;

        cpu.text = cpuCount.ToString();
        gpu.text = gpuCount.ToString();
        ram.text = ramCount.ToString();
    }

    void AddTask(int type)
    {
        //depending on the taskType given add the new Task
        // task types:
        // 0 - explosion,
        // 1 - enemy,
        // 2 - movement,
        // 3 - browser (streaming),

        Task newTask = new Task();
        switch (type)
        {
            case 0:
                newTask.maxTime = 5;
                newTask.diff = 3;
                newTask.resources = "GPU";
                break;
            case 1:
                newTask.maxTime = 15;
                newTask.diff = 2;
                newTask.resources = "CPU + RAM";
                break;
            case 2:
                newTask.maxTime = 10;
                newTask.diff = 1;
                newTask.resources = "CPU";
                break;
            case 3:
                newTask.maxTime = 15;
                newTask.diff = 2;
                newTask.resources = "RAM";
                break;
        }
        Tasks.Add(newTask);
    }

    IEnumerator TaskTimer()
    {
        //select a random taskType and pass it to AddTask
        AddTask(Random.Range(0, 3));
        yield return new WaitForSeconds(10);
        StartCoroutine("TaskTimer");
    }

    IEnumerator ResourceRefresher()
    {
        //adds one to each resource at the same time because why should we do it otherwise? 

        cpuCount++;
        ramCount++;
        gpuCount++;
    
        yield return new WaitForSeconds(resourceRefreshRate);
        StartCoroutine("ResourceRefresher");
    }
}
