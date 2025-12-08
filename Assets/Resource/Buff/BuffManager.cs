using System.Collections.Generic;
using UnityEngine;
using static Buff;
using static SkillBuff;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;

    public List<Buff> allBuff;
    private const int buffMaxIndex = 10;


    private void Awake()
    {
        instance = this;
    }

    private void CreateBuff()
    {
        allBuff = new List<Buff>();
        for (int i = 0; i <= buffMaxIndex; i++)
        {
            var buff = NewBuff();
            allBuff.Add(buff);
            Debug.Log($"{buff.name},{buff.level},{buff.type.ToString()}");
        }
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] buffer = new char[length];

        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[Random.Range(0, chars.Length)];
        }
        string name = new string(buffer);

        return name += "[Value Buff]";
    }

    private Buff NewBuff()
    {
        Buff buff = new Buff();
        int type = Random.Range(0, 3);
        if (type == 0)
        {
            buff.BuffInit(RandomString(3), 1, 5, BuffType.ValueOnly);
            buff.ValueBuffInit(40, 10, 20);

        }
        else if (type == 1)
        {
            buff.BuffInit(RandomString(4), 1, 4, BuffType.SkillOnly);
            buff.SkillBuffInit(AttackElements.Ice);
        }
        else if (type == 2)
        {
            buff.BuffInit(RandomString(5), 1, 3, BuffType.Both);
            buff.ValueBuffInit(100, 10, 1);
            buff.SkillBuffInit(AttackElements.Fire);
        }

        return buff;
    }

    public Buff RendomBuff() => allBuff[Random.Range(0, allBuff.Count)];

    void Start()
    {
        CreateBuff();
    }
}
