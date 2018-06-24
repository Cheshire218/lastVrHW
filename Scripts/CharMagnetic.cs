using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMagnetic : MonoBehaviour
{
    [SerializeField] float SpellDistance = 20;                           // Дистанция снаряда магнита
    [SerializeField] MagnitePoint MagniteSpell;                     // Работаем с нашей структурой
    [SerializeField] Transform BlueHolder, RedHolder;         // Вспомогательные объекты
    [SerializeField] Material RedMat, BlueMat, YellowMat;  // Ссылки на материалы для частиц
    [SerializeField] ParticleSystem hl_Reference;              // Ссылка на систему частиц

    void SetBlue(Transform trans) // Вызываем, если попали по движимому объекту 
    {
        MagniteSpell.BlueObj = trans;                 // Делаем ссылку, как на активный объект
        MagniteSpell.BluePos = trans.position;  // Ссылка на точку для проверки расстояния
        Higlighting(true, trans);                          // Создание подсветки 
        CheckToJoint();                                    // Проверка можно ли уже создавать сочленение
    }

    void SetRed(Transform trans)  // Вызываем, если попали по движимому объекту 
    {
        MagniteSpell.RedObj = trans;
        MagniteSpell.RedPos = trans.position;
        Higlighting(false, trans);
        CheckToJoint();
    }

    void SetBlue(Vector3 trans)  // Вызываем, если попали по недвижимому объекту
    {
        MagniteSpell.BlueObj = BlueHolder;  // Делаем ссылку, как на “заглушку”
        MagniteSpell.BluePos = trans;          // Ссылка на точку для проверки расстояния 
        BlueHolder.position = trans;             // Перемещаем “заглушку” в точку, где она должна быть
        BlueHolder.GetChild(0).gameObject.SetActive(true); // Включаем подсветку
        CheckToJoint();   // Проверка можно ли уже создавать сочленение
    }

    void SetRed(Vector3 trans)  // Вызываем, если попали по недвижимому объекту
    {
        MagniteSpell.RedObj = RedHolder;
        MagniteSpell.RedPos = trans;
        RedHolder.position = trans;
        RedHolder.GetChild(0).gameObject.SetActive(true);
        CheckToJoint();
    }

    void CheckToJoint()
    {
        if (MagniteSpell.BlueObj != null && MagniteSpell.RedObj != null)
        {
            if (Vector3.Distance(MagniteSpell.RedPos, MagniteSpell.BluePos) < SpellDistance) CreateJoint();
            else EreaseSpell();
        }
    }

    void CreateJoint()
    {
        SpringJoint sp = MagniteSpell.BlueObj.gameObject.AddComponent<SpringJoint>(); // Создание joint’а
        sp.autoConfigureConnectedAnchor = false; // Чтобы joint всё не сломал, выключаем авто-якоря.
        Vector3 vec = new Vector3(0, 0, 0);
        sp.anchor = vec;  // Обнуляем наш якорь до центра объекта
        sp.connectedAnchor = vec;  // Обнуляем якорь объекта-цели до центра
        sp.enableCollision = true;  // Включаем коллизию между объектами сочленения.
        sp.enablePreprocessing = false;  // Может вызвать неприятные дёргания, так что выключаем

        // Скрепливаем первый объект со вторым
        sp.connectedBody = MagniteSpell.RedObj.GetComponent<Rigidbody>();

        EreaseSpell();                              // Очищаем все ссылки на активные объекты
        MagniteSpell.JointList.Add(sp);  // Добавляем ссылку на SpringJoint в лист

        //  Добавляем ссылку на Rigidbody объектов
        Rigidbody rg = sp.GetComponent<Rigidbody>();
        AddRG(sp.connectedBody);
        AddRG(rg);

    }

    void AddRG(Rigidbody RG)
    {
        for (int i = 0; i < MagniteSpell.RG.Count; i++)
        {
            if (RG == MagniteSpell.RG[i]) break;

            if (i == MagniteSpell.RG.Count - 1) MagniteSpell.RG.Add(RG);
        }
    }

    void Higlighting(bool IsBlue, Transform trans)
    {
        ParticleSystem ps = Instantiate(hl_Reference, trans, false);

        if (IsBlue) ps.GetComponent<Renderer>().material = BlueMat;
        else ps.GetComponent<Renderer>().material = RedMat;

        MagniteSpell.HighLight.Add(ps);
    }

    void EreaseSpell()
    {
        MagniteSpell.BlueObj = null;
        MagniteSpell.RedObj = null;

        for (int i = 0; i < MagniteSpell.HighLight.Count; i++)
            MagniteSpell.HighLight[i].GetComponent<Renderer>().material = YellowMat;
    }

    void DestroyAllJoints()
    {
        for (int i = 0; i < MagniteSpell.JointList.Count; i++)
        {
            Destroy(MagniteSpell.JointList[i]);
        }

        for (int i = 0; i < MagniteSpell.RG.Count; i++)
        {
            MagniteSpell.RG[i].angularDrag = 0.05f;
            MagniteSpell.RG[i].drag = 0;
            MagniteSpell.RG[i].WakeUp();
        }

        MagniteSpell.JointList.Clear();
        MagniteSpell.RG.Clear();
        EreaseSpell(); // не менять местами с циклом

        for (int i = 0; i < MagniteSpell.HighLight.Count; i++)
            Destroy(MagniteSpell.HighLight[i]);

        MagniteSpell.HighLight.Clear();
        DisableHolders();
    }

    void DisableHolders()
    {
        BlueHolder.GetChild(0).gameObject.SetActive(false);
        RedHolder.GetChild(0).gameObject.SetActive(false);
    }

    void setUpParticles(ParticleSystem ps)
    {
        var Part = ps.GetComponent<Renderer>();
        Part.material = BlueMat;
    }



}
