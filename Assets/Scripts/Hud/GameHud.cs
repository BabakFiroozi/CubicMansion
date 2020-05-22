using System;
using System.Collections;
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
            StartCoroutine(_Start());
        }

        IEnumerator _Start()
        {
            yield return new WaitWhile(()=> PlayerCharacter.Instance == null || PlayerCharacter.Instance.Movement.Unit.CurrentWeapon == null);
            WeaponEquipped();
            PlayerCharacter.Instance.Movement.Unit.WeaponEquipped += WeaponEquipped;
        }

        void OnDestroy()
        {
            PlayerCharacter.Instance.Movement.Unit.WeaponEquipped -= WeaponEquipped;
        }

        void WeaponEquipped()
        {
            _currentWeaponText.text = PlayerCharacter.Instance.Movement.Unit.CurrentWeapon.WeaponName;
        }
    }
}