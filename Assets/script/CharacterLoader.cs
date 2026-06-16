using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public GameObject[] playableCharacters;
    public AnimatorOverrideController[] overrideControllers; // 每个角色对应的动画重写控制器

    void Awake()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Debug.Log("Loaded character index: " + index);

        for (int i = 0; i < playableCharacters.Length; i++)
        {
            bool active = (i == index);
            playableCharacters[i].SetActive(active);

            // 为激活的角色设置对应的 Animator Override Controller
            if (active && overrideControllers != null && i < overrideControllers.Length)
            {
                Animator anim = playableCharacters[i].GetComponent<Animator>();
                if (anim != null && overrideControllers[i] != null)
                {
                    anim.runtimeAnimatorController = overrideControllers[i];
                }
            }
        }
    }
}