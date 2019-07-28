using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SkillTreeReader : MonoBehaviour {

	#region SINGLETON
	private static SkillTreeReader _instance;

    public static SkillTreeReader Instance
    {
        get
        {
            return _instance;
        }
        set
        {
        }
    }

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
			SetUpSkillTree();
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
	#endregion

	// 모든 스킬들을 가지는 스킬어레이
	private Skill[] skillTrees;

    // 스킬트리 내부의 스킬들
    public Dictionary<int, Skill> _skills;

	string jsonName = "skilltree.json";

    public Skill skillInspected;

    public int availablePoints = 100;



	// 스킬트리 셋업
	void SetUpSkillTree ()
    {
        _skills = new Dictionary<int, Skill>();

        LoadSkillTree();
	}

    public void LoadSkillTree()
    {
		string s = string.Format("Assets/SkillTree/Data/" + jsonName);

        string path = s;
        string dataAsJson;
        if (File.Exists(path))
        {
            dataAsJson = File.ReadAllText(path);

            SkillTree loadedData = JsonUtility.FromJson<SkillTree>(dataAsJson);

            skillTrees = new Skill[loadedData.skilltree.Length];
            skillTrees = loadedData.skilltree;

            for (int i = 0; i < skillTrees.Length; ++i)
            {
                _skills.Add(skillTrees[i].id_Skill, skillTrees[i]);
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }        
    }

    public bool IsSkillUnlocked(int id_skill)
    {
        if (_skills.TryGetValue(id_skill, out skillInspected))
        {
            return skillInspected.unlocked;
        }
        else
        {
            return false;
        }
    }

    public bool CanSkillBeUnlocked(int id_skill)
    {
        bool canUnlock = true;
        if(_skills.TryGetValue(id_skill, out skillInspected)) 
        {
            if(skillInspected.cost <= availablePoints) 
            {
                int[] dependencies = skillInspected.skill_Dependencies;
                for (int i = 0; i < dependencies.Length; ++i)
                {
                    if (_skills.TryGetValue(dependencies[i], out skillInspected))
                    {
                        if (!skillInspected.unlocked)
                        {
                            canUnlock = false;
                            break;
                        }
                    }
                    else 
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            
        }
        else 
        {
            return false;
        }
        return canUnlock;
    }

    public bool UnlockSkill(int id_Skill)
    {
        if(_skills.TryGetValue(id_Skill, out skillInspected))
        {
            if (skillInspected.cost <= availablePoints)
            {
                availablePoints -= skillInspected.cost;
                skillInspected.unlocked = true;

                _skills.Remove(id_Skill);
                _skills.Add(id_Skill, skillInspected);

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
