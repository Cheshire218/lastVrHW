using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MagnitePoint
{
    public List<SpringJoint> JointList;           // Список всех существующих джоинтов
    public List<Rigidbody> RG;                    // Список всех связанных Rigidbody
    public List<ParticleSystem> HighLight; // подсветка с помощью частиц
    public Transform BlueObj, RedObj;       // Ссылки на активные объекты помеченные цветом
    public Vector3 BluePos, RedPos;        // Ссылка на точку для измерения расстояния
}
