using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CharacterChange : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] characters;
    public Button returnButton;
    public Button character1;
    public Button character2;
    public Button character3;
    int selectedCharacterIndex;
    void Start()
    {
        returnButton.onClick.AddListener(Fanhui);

        character1.onClick.AddListener(Character1);
        character2.onClick.AddListener(Character2);
        character3.onClick.AddListener(Character3);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Fanhui()
    {
        Debug.Log(selectedCharacterIndex);
     /*   SceneManager.LoadScene("runScene");*/
        // 1. 把角色编号存到 PlayerPrefs
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacterIndex);

        // 2. 强制保存到磁盘（防止切换太快丢数据）
        PlayerPrefs.Save();

        // 3. 控制台打印编号，方便调试
        Debug.Log(selectedCharacterIndex);

        // 4. 切换到游戏场景
        SceneManager.LoadScene(0);
    }

    private void Character1()
    {
        characters[0].SetActive(true);
        characters[1].SetActive(false);
        characters[2].SetActive(false);
        selectedCharacterIndex = 0;
    }
    private void Character2()
    {
        characters[0].SetActive(false);
        characters[1].SetActive(true);
        characters[2].SetActive(false);
        selectedCharacterIndex = 1;
    }
    private void Character3()
    {
        characters[0].SetActive(false);
        characters[1].SetActive(false);
        characters[2].SetActive(true);
        selectedCharacterIndex = 2;
    }

}
