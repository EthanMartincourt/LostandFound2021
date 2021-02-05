using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;
    [SerializeField] private float timeLimit;
    [SerializeField]
    private HiddenObjectHolder hiddenObjectHolder;

    private List<HiddenObjectData> activeHiddenObjectsList;

    [SerializeField]
    private int maxActiveHiddenObjectsCount = 5;

    private int totalHiddenObjectsFound = 0;
    private float currentTime = 0;
    private GameStatus gameStatus = GameStatus.NEXT;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(gameObject);
    }

    private void Start()
    {
        activeHiddenObjectsList = new List<HiddenObjectData>();
        AssignHiddenObjects();
    }
    void AssignHiddenObjects()
    {
        currentTime = timeLimit;
        UIManager.instance.TimerText.text = "" + currentTime;
        totalHiddenObjectsFound = 0;
        activeHiddenObjectsList.Clear();
        for (int i = 0; i < hiddenObjectHolder.HiddenObjectsList.Count; i++)
        {
            hiddenObjectHolder.HiddenObjectsList[i].hiddenObject.GetComponent<Collider2D>().enabled = false;
        }

        //local variable to keep the count
        int k = 0;
        while (k < maxActiveHiddenObjectsCount)
        {
            int randomValue = UnityEngine.Random.Range(0, hiddenObjectHolder.HiddenObjectsList.Count);

            if (!hiddenObjectHolder.HiddenObjectsList[randomValue].makeHidden)
            {
                hiddenObjectHolder.HiddenObjectsList[randomValue].hiddenObject.name = "" + k;
                hiddenObjectHolder.HiddenObjectsList[randomValue].makeHidden = true;
                hiddenObjectHolder.HiddenObjectsList[randomValue].hiddenObject.GetComponent<Collider2D>().enabled = true;

                activeHiddenObjectsList.Add(hiddenObjectHolder.HiddenObjectsList[randomValue]);

                k++;
            }
        }

        UIManager.instance.PopulateHiddenObjectIcon(activeHiddenObjectsList);
        gameStatus = GameStatus.PLAYING;
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {


            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);
                SFxManager.sfxInstance.Audio.PlayOneShot(SFxManager.sfxInstance.Click);

                if (hit && hit.collider != null)
                {
                    //Debug.Log("Object Name:" + hit.collider.gameObject.name);

                    hit.collider.gameObject.SetActive(false);
                    UIManager.instance.CheckSelectedHiddenObject(hit.collider.gameObject.name);

                    for (int i = 0; i < activeHiddenObjectsList.Count; i++)
                    {
                        if (activeHiddenObjectsList[i].hiddenObject.name == hit.collider.gameObject.name)
                        {
                            activeHiddenObjectsList.RemoveAt(i);
                            break;

                        }
                    }

                    totalHiddenObjectsFound++;

                    if (totalHiddenObjectsFound >= maxActiveHiddenObjectsCount)
                    {
                        Debug.Log("Level Complete");
                        UIManager.instance.GamePanel.SetActive(true);
                        gameStatus = GameStatus.NEXT;
                    }
                }
            }

            currentTime -= Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            UIManager.instance.TimerText.text = time.ToString("mm' : 'ss");

            if (currentTime <= 0)
            {
                Debug.Log("Level Lost");
                UIManager.instance.GamePanel.SetActive(true);
                gameStatus = GameStatus.NEXT;
            }
        }
    }
    public IEnumerator HintMethod()
    {
        int randomValue = UnityEngine.Random.Range(0, activeHiddenObjectsList.Count);
        Vector3 originalScale = activeHiddenObjectsList[randomValue].hiddenObject.transform.localScale;
        activeHiddenObjectsList[randomValue].hiddenObject.transform.localScale = originalScale * 1.25f;
        yield return new WaitForSeconds(0.25f);
        activeHiddenObjectsList[randomValue].hiddenObject.transform.localScale = originalScale;
    }
}

//List to store all objects.
[System.Serializable]
public class HiddenObjectData
{
    public string name;
    public GameObject hiddenObject;
    public bool makeHidden = false;
}

public enum GameStatus
{
    PLAYING,
    NEXT
}