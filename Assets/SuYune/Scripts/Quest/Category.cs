using System.Collections;
using System.Collections.Generic;
using System;   // IEquatable<T> 인터페이스를 받기 위함
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
        // other가 null이면
        if (other is null)
        {
            return false;
        }
        // Reference 비교해서 other와 자신이 같다면
        if (ReferenceEquals(other, this))
        {
            return true;
        }
        // 자신과 other의 타입이 다르면
        if (GetType() != other.GetType())
        {
            return false;
        }

        // 자신의 codeName과 other의 codeName이 같은지 비교 후 리턴
        return codeName == other.CodeName;
    }

    public override int GetHashCode() => (CodeName, DisplayName).GetHashCode();

    public override bool Equals(object other) => base.Equals(other);

    // string 비교연산자
    public static bool operator == (Category lhs, string rhs)
    {
        if (lhs is null)
        {
            return ReferenceEquals(rhs, null);
        }
        return lhs.CodeName == rhs || lhs.DisplayName == rhs; // true 리턴
    }

    // 대칭연산자
    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
    // category.Codename == "Kill"  이라고 쓸 필요 없이
    // category == "Kill"   이라고 쓰면 됨
    #endregion
}
