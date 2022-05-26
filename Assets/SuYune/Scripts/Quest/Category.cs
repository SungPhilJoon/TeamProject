using System.Collections;
using System.Collections.Generic;
using System;   // IEquatable<T> �������̽��� �ޱ� ����
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Categoty", fileName = "Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;

    public string CodeName => codeName;
    public string DisplayName => displayName;

    #region Operator
    public bool Equals(Category other)
    {
        // other�� null�̸�
        if (other is null)
        {
            return false;
        }
        // Reference ���ؼ� other�� �ڽ��� ���ٸ�
        if (ReferenceEquals(other, this))
        {
            return true;
        }
        // �ڽŰ� other�� Ÿ���� �ٸ���
        if (GetType() != other.GetType())
        {
            return false;
        }

        // �ڽ��� codeName�� other�� codeName�� ������ �� �� ����
        return codeName == other.CodeName;
    }

    public override int GetHashCode() => (CodeName, DisplayName).GetHashCode();

    public override bool Equals(object other) => base.Equals(other);

    // string �񱳿�����
    public static bool operator == (Category lhs, string rhs)
    {
        if (lhs is null)
        {
            return ReferenceEquals(rhs, null);
        }
        return lhs.CodeName == rhs || lhs.DisplayName == rhs; // true ����
    }

    // ��Ī������
    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
    // category.Codename == "Kill"  �̶�� �� �ʿ� ����
    // category == "Kill"   �̶�� ���� ��
    #endregion
}
