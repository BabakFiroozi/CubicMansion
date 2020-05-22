using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CubicMansion
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _currentWeaponText;

        void Start()
        {
            PlayerCharacter.Instance.Movement.Unit.WeaponEquipped += WeaponEquipped;
        }

        void WeaponEquipped()
        {
            _currentWeaponText.text = PlayerCharacter.Instance.Movement.Unit.CurrentWeapon.WeaponName;
        }
    }
}